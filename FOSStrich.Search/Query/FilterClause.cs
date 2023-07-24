namespace FOSStrich.Search;

public class FilterClause
{
    protected internal FilterClause()
    {
        Operation = BooleanOperation.And;
        SubClauses = new[] { this };
    }

    // TODO : De-duplication of identical FilterParameters.
    private FilterClause(BooleanOperation operation, FilterClause[] subClauses)
    {
        Operation = operation;
        SubClauses = subClauses;
    }

    public BooleanOperation Operation { get; }
    public IEnumerable<FilterClause> SubClauses { get; }

    internal IEnumerable<IFilterParameter> AllFilterParameters() =>
        SubClauses.SelectMany(clause =>
        {
            var parameter = clause as IFilterParameter;

            return parameter != null
                ? new[] { parameter }
                : clause.AllFilterParameters();
        });

    // http://stackoverflow.com/questions/15439864/how-do-i-override-logical-and-operator
    public static bool operator false(FilterClause clause) { return false; }

    public static FilterClause operator &(FilterClause leftClause, FilterClause rightClause)
    {
        if (leftClause == null)
            throw new ArgumentNullException(nameof(leftClause));

        if (rightClause == null)
            throw new ArgumentNullException(nameof(rightClause));

        // Flatten when possible
        switch (leftClause.Operation)
        {
            case BooleanOperation.And:
                return rightClause.Operation == BooleanOperation.And
                    ? new FilterClause(BooleanOperation.And, leftClause.SubClauses.Concat(rightClause.SubClauses).ToArray())
                    : new FilterClause(BooleanOperation.And, leftClause.SubClauses.Concat(new[] { rightClause }).ToArray());

            case BooleanOperation.Or:
                return rightClause.Operation == BooleanOperation.And
                    ? new FilterClause(BooleanOperation.And, rightClause.SubClauses.Concat(new[] { leftClause }).ToArray())
                    : new FilterClause(BooleanOperation.And, new[] { leftClause, rightClause });

            default:
                return new FilterClause(BooleanOperation.And, new[] { leftClause, rightClause });
        }
    }

    public static bool operator true(FilterClause clause) { return false; }

    public static FilterClause operator |(FilterClause leftClause, FilterClause rightClause)
    {
        if (leftClause == null)
            throw new ArgumentNullException(nameof(leftClause));

        if (rightClause == null)
            throw new ArgumentNullException(nameof(rightClause));

        // Flatten when possible
        switch (leftClause.Operation)
        {
            case BooleanOperation.And:
                return rightClause.Operation == BooleanOperation.Or
                    ? new FilterClause(BooleanOperation.Or, rightClause.SubClauses.Concat(new[] { leftClause }).ToArray())
                    : new FilterClause(BooleanOperation.Or, new[] { leftClause, rightClause });

            case BooleanOperation.Or:
                return rightClause.Operation == BooleanOperation.Or
                    ? new FilterClause(BooleanOperation.Or, leftClause.SubClauses.Concat(rightClause.SubClauses).ToArray())
                    : new FilterClause(BooleanOperation.Or, leftClause.SubClauses.Concat(new[] { rightClause }).ToArray());

            default:
                return new FilterClause(BooleanOperation.Or, new[] { leftClause, rightClause });
        }
    }

    // TODO : Optimize?
    public static FilterClause operator !(FilterClause clause)
    {
        if (clause == null)
            throw new ArgumentNullException(nameof(clause));

        return new FilterClause(BooleanOperation.Not, new[] { clause });
    }
}

public enum BooleanOperation
{
    And,
    Or,
    Not
}