namespace LdesReader;

public class DirectTriples
{
    public Triple[] Triples { get; set; }

    public Triple? GetTripleByPredicate(string predicate)
    {
        return Triples.SingleOrDefault(x => x.Predicate == predicate);
    }
}
