namespace SoftwareBotany.Sunlight
{
    internal interface IVectorLogic
    {
        void Decompress(Word[] iWords, Word[] jWords, int jWordCountPhysical);
        void And(Word[] iWords, ref int iWordCountPhysical, ref int iWordCountLogical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical);
        int AndPopulation(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical);
        void Or(Word[] iWords, int iWordCountPhysical, bool jIsCompressed, Word[] jWords, int jWordCountPhysical);
    }
}