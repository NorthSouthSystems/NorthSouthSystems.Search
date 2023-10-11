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
            clause is IFilterParameter parameter
                ? new[] { parameter }
                : clause.AllFilterParameters());

    // http://stackoverflow.com/questions/15439864/how-do-i-override-logical-and-operator
    public static bool operator false(FilterClause clause) => false;

    public static FilterClause operator &(FilterClause leftClause, FilterClause rightClause)
    {
        if (leftClause == null)
            throw new ArgumentNullException(nameof(leftClause));

        if (rightClause == null)
            throw new ArgumentNullException(nameof(rightClause));

        // Flatten when possible
        return leftClause.Operation switch
        {
            BooleanOperation.And =>
                rightClause.Operation == BooleanOperation.And
                    ? new(BooleanOperation.And, leftClause.SubClauses.Concat(rightClause.SubClauses).ToArray())
                    : new(BooleanOperation.And, leftClause.SubClauses.Concat(new[] { rightClause }).ToArray()),

            BooleanOperation.Or =>
                rightClause.Operation == BooleanOperation.And
                    ? new(BooleanOperation.And, rightClause.SubClauses.Concat(new[] { leftClause }).ToArray())
                    : new(BooleanOperation.And, new[] { leftClause, rightClause }),

            _ => new(BooleanOperation.And, new[] { leftClause, rightClause })
        };
    }

    public static bool operator true(FilterClause clause) => false;

    public static FilterClause operator |(FilterClause leftClause, FilterClause rightClause)
    {
        if (leftClause == null)
            throw new ArgumentNullException(nameof(leftClause));

        if (rightClause == null)
            throw new ArgumentNullException(nameof(rightClause));

        // Flatten when possible
        return leftClause.Operation switch
        {
            BooleanOperation.And =>
                rightClause.Operation == BooleanOperation.Or
                    ? new(BooleanOperation.Or, rightClause.SubClauses.Concat(new[] { leftClause }).ToArray())
                    : new(BooleanOperation.Or, new[] { leftClause, rightClause }),

            BooleanOperation.Or =>
                rightClause.Operation == BooleanOperation.Or
                    ? new(BooleanOperation.Or, leftClause.SubClauses.Concat(rightClause.SubClauses).ToArray())
                    : new(BooleanOperation.Or, leftClause.SubClauses.Concat(new[] { rightClause }).ToArray()),

            _ => new(BooleanOperation.Or, new[] { leftClause, rightClause })
        };
    }

    // TODO : Optimize?
    public static FilterClause operator !(FilterClause clause)
    {
        if (clause == null)
            throw new ArgumentNullException(nameof(clause));

        return new(BooleanOperation.Not, new[] { clause });
    }
}

public enum BooleanOperation
{
    And,
    Or,
    Not
}