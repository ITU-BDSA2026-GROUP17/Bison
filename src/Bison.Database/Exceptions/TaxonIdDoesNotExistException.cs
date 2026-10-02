namespace Bison.Database.Exceptions;

public class TaxonIdDoesNotExistException(string taxonId) : Exception($"Taxon with ID {taxonId} does not exist")
{
    public readonly string TaxonId = taxonId;
}
