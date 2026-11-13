using Bison.Database.Exceptions;
using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database;

public class DatabaseRepository(BisonDbContext dbContext) : IDatabaseRepository
{
    private readonly BisonDbContext _dbContext = dbContext;

    public async Task<List<Taxon>> GetTaxons(int? limit = null, int? skip = null)
    {
        IQueryable<Taxon> query = _dbContext.Taxons
            .OrderByDescending(t => t.Id);
        if (skip is int skipAmount)
        {
            query = query.Skip(skipAmount);
        }
        if (limit is int limitAmount)
        {
            query = query.Take(limitAmount);
        }
        return await query.ToListAsync();
    }
    public async Task<Taxon?> GetTaxonByDWCId(string taxonId)
    {
        return await _dbContext.Taxons
            .Where(t => t.DWC_TaxonId == taxonId)
            .FirstOrDefaultAsync();
    }
    public async Task<Taxon?> GetTaxonByNumericId(int taxonId)
    {
        return await _dbContext.Taxons.FindAsync(taxonId);
    }

    public async Task<User> CreateUser(string username, string email, string name)
    {
        var res = _dbContext.Users.Add(new User(username, email, name));
        await _dbContext.SaveChangesAsync();
        return res.Entity;
    }
    public async Task<User?> GetUserById(string userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }
    public async Task<User?> GetUserByUserName(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<Observation?> GetObservation(int id)
    {
        return await _dbContext.Posts
            .Include("Taxon")
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync() is Observation obs ? obs : null;
    }
    public async Task<List<Observation>> GetObservations(int? limit = null, int? skip = null)
    {
        IQueryable<Observation> query = _dbContext.Posts
            .Include("Taxon")
            .Where(p => p is Observation)
            .Select(p => (Observation)p)
            .OrderByDescending(obs => obs.Id);
        if (skip is int skipAmount)
        {
            query = query.Skip(skipAmount);
        }
        if (limit is int limitAmount)
        {
            query = query.Take(limitAmount);
        }
        return await query.ToListAsync();
    }
    public async Task<List<Observation>> GetObservationsByUser(string userId, int? limit = null, int? skip = null)
    {
        IQueryable<Observation> query = _dbContext.Posts
            .Include("Taxon")
            .Where(p => p is Observation)
            .Select(p => (Observation)p)
            .Where(obs => obs.Author.Id == userId)
            .OrderByDescending(obs => obs.Id);
        if (skip is int skipAmount)
        {
            query = query.Skip(skipAmount);
        }
        if (limit is int limitAmount)
        {
            query = query.Take(limitAmount);
        }
        return await query.ToListAsync();
    }
    public async Task<Observation> CreateObservation(string userId, string taxonId, string observation)
    {
        var author = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserDoesNotExistException(userId);
        var taxon = await GetTaxonByDWCId(taxonId) ?? throw new TaxonIdDoesNotExistException(taxonId);

        var res = _dbContext.Posts.Add(new Observation()
        {
            Author = author,
            Text = observation,
            Taxon = taxon
        });
        await _dbContext.SaveChangesAsync();
        return (Observation)res.Entity;
    }

    public async Task<List<Comment>> GetCommentsForObservation(int observationId, int? limit = null, int? skip = null)
    {
        IQueryable<Comment> query = _dbContext.Posts
            .Include("Observation")
            .Where(p => p is Comment)
            .Select(p => (Comment)p)
            .Where(cmt => cmt.Observation.Id == observationId)
            .OrderByDescending(cmt => cmt.Id);
        if (skip is int skipAmount)
        {
            query = query.Skip(skipAmount);
        }
        if (limit is int limitAmount)
        {
            query = query.Take(limitAmount);
        }
        return await query.ToListAsync();
    }
    public async Task<Comment> CreateComment(string authorId, int observationId, string comment)
    {
        var author = await _dbContext.Users.FindAsync(authorId)
            ?? throw new UserDoesNotExistException(authorId);
        var observation = await _dbContext.Posts
            .Where(p => p is Observation && p.Id == observationId)
            .Select(p => (Observation)p)
            .FirstOrDefaultAsync()
            ?? throw new ObservationDoesNotExistException(observationId);

        var res = _dbContext.Posts.Add(new Comment()
        {
            Author = author,
            Observation = observation,
            Text = comment
        });
        await _dbContext.SaveChangesAsync();
        return (Comment)res.Entity;
    }

    public async Task<List<Proposal>> GetProposalsForObservation(int observationId, int? limit = null, int? skip = null)
    {
        IQueryable<Proposal> query = _dbContext.Posts
            .Include("Taxon")
            .Include("Observation")
            .Where(p => p is Proposal)
            .Select(p => (Proposal)p)
            .Where(ppl => ppl.Observation.Id == observationId)
            .OrderByDescending(ppl => ppl.Id);
        if (skip is int skipAmount)
        {
            query = query.Skip(skipAmount);
        }
        if (limit is int limitAmount)
        {
            query = query.Take(limitAmount);
        }
        return await query.ToListAsync();
    }
    public async Task<Proposal> CreateProposal(string authorId, int observationId, string taxonId, string text)
    {
        var author = await _dbContext.Users.FindAsync(authorId)
            ?? throw new UserDoesNotExistException(authorId);
        var observation = await _dbContext.Posts
            .Where(p => p is Observation && p.Id == observationId)
            .Select(p => (Observation)p)
            .FirstOrDefaultAsync()
            ?? throw new ObservationDoesNotExistException(observationId);
        var taxon = await GetTaxonByDWCId(taxonId) ?? throw new TaxonIdDoesNotExistException(taxonId);

        var res = _dbContext.Posts.Add(new Proposal()
        {
            Author = author,
            Observation = observation,
            Taxon = taxon,
            Text = text,
        });
        await _dbContext.SaveChangesAsync();
        return (Proposal)res.Entity;
    }
}
