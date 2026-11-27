using Bison.Database.Exceptions;
using Bison.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
    public async Task<Taxon?> GetTaxonByVernacularName(string vernacularName)
    {
        return await _dbContext.Taxons
            .Where(t => t.VernacularName == vernacularName)
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

    public async Task<Post?> GetPost(int id)
    {
        return await _dbContext.Posts
            .Include("Observation")
            .Include("Taxon")
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Observation?> GetObservation(int id)
    {
        return await _dbContext.Posts
            .Include("Taxon")
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync() is Observation obs ? obs : null;
    }

    public async Task EditObservation(int obsId, string? newTaxonId = null, string? newText = null, string? newPicturePath = null)
    {
        var obs = await GetObservation(obsId) ?? throw new ObservationDoesNotExistException(obsId); // observation does not exist

        if (newTaxonId != null)
        {
            var newTaxon = await GetTaxonByDWCId(newTaxonId);
            if (newTaxon != null)
            {
                obs.Taxon = newTaxon;
            }
        }
        if (newText != null)
        {
            obs.Text = newText;
        }
        obs.PicturePath = newPicturePath; //picture path is allowed to be null
        await _dbContext.SaveChangesAsync();
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
    public async Task<Observation> CreateObservation(string userId, string taxonId, string observation, string? picturePath = null)
    {
        var author = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserDoesNotExistException(userId);
        var taxon = await GetTaxonByDWCId(taxonId) ?? throw new TaxonIdDoesNotExistException(taxonId);

        var res = _dbContext.Posts.Add(new Observation()
        {
            Author = author,
            Text = observation,
            Taxon = taxon,
            PicturePath = picturePath
        });
        await _dbContext.SaveChangesAsync();
        return (Observation)res.Entity;
    }

    public async Task<List<Post>> GetSubpostsForObservation(int observationId, int? limit = null, int? skip = null)
    {
        IQueryable<Post> query = _dbContext.Posts
            .Include("Observation")
            .Include("Taxon")
            .Where(p =>
                (p is Comment && ((Comment)p).Observation.Id == observationId) ||
                (p is Proposal && ((Proposal)p).Observation.Id == observationId)
            )
            .OrderBy(p => p.Id);
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
    public async Task EditComment(int commentId, string? newText = null)
    {
        var com = (Comment?)await GetPost(commentId) ?? throw new PostDoesNotExistException(commentId);

        if (newText != null)
        {
            com.Text = newText;
        }
        await _dbContext.SaveChangesAsync();
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
    public async Task EditProposal(int proposalId, string? newTaxonId = null, string? newText = null)
    {
        var ppl = (Proposal?)await GetPost(proposalId) ?? throw new PostDoesNotExistException(proposalId);

        if (newTaxonId != null)
        {
            var newTaxon = await GetTaxonByDWCId(newTaxonId);
            if (newTaxon != null)
            {
                ppl.Taxon = newTaxon;
            }
        }
        if (newText != null)
        {
            ppl.Text = newText;
        }
        await _dbContext.SaveChangesAsync();
    }
}
