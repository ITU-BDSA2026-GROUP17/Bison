using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database;

public class DatabaseRepository(BisonDBContext dbContext) : IDatabaseRepository
{
    private readonly BisonDBContext _dbContext = dbContext;

    public async Task<User> CreateUser(string name)
    {
        var res = _dbContext.Users.Add(new User() { Name = name });
        await _dbContext.SaveChangesAsync();
        return res.Entity;
    }
    public async Task<User?> GetUserById(int userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }
    public async Task<User?> GetUserByName(string name)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<Observation?> GetObservation(int id)
    {
        return await _dbContext.Observations.FindAsync(id);
    }
    public async Task<List<Observation>> GetObservations(int? limit = null, int? skip = null)
    {
        IQueryable<Observation> query = _dbContext.Observations;
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
    public async Task<List<Observation>> GetObservationsByUser(int userId, int? limit = null, int? skip = null)
    {
        IQueryable<Observation> query = _dbContext.Observations.Where(obs => obs.Author.Id == userId);
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
    public async Task<Observation> CreateObservation(int userId, string observation, string location)
    {
        var author = await _dbContext.Users.FindAsync(userId);
        if (author is User authorNotNull)
        {
            var res = _dbContext.Observations.Add(new Observation()
            {
                Author = authorNotNull,
                Text = observation,
                Location = location
            });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }
        else
        {
            throw new UserDoesNotExistException(userId);
        }
    }

    public async Task<List<Comment>> GetCommentsForObservation(int observationId, int? limit = null, int? skip = null)
    {
        IQueryable<Comment> query = _dbContext.Comments.Where(cmt => cmt.Observation.Id == observationId);
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
    public async Task<Comment> CreateComment(int authorId, int observationId, string comment)
    {
        var author = await _dbContext.Users.FindAsync(authorId);
        if (author is User authorNotNull)
        {
            var observation = await _dbContext.Observations.FindAsync(observationId);
            if (observation is Observation observationNotNull)
            {
                var res = _dbContext.Comments.Add(new Comment()
                {
                    Author = authorNotNull,
                    Observation = observationNotNull,
                    Text = comment
                });
                await _dbContext.SaveChangesAsync();
                return res.Entity;
            }
            else
            {
                throw new ObservationDoesNotExistException(observationId);
            }
        }
        else
        {
            throw new UserDoesNotExistException(authorId);
        }
    }

    public async Task<List<Proposal>> GetProposalsForObservation(int observationId, int? limit = null, int? skip = null)
    {
        IQueryable<Proposal> query = _dbContext.Proposals.Where(ppl => ppl.Observation.Id == observationId);
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
    public async Task<Proposal> CreateProposal(int authorId, int observationId, string taxonId)
    {
        var author = await _dbContext.Users.FindAsync(authorId);
        if (author is User authorNotNull)
        {
            var observation = await _dbContext.Observations.FindAsync(observationId);
            if (observation is Observation observationNotNull)
            {
                var res = _dbContext.Proposals.Add(new Proposal()
                {
                    Author = authorNotNull,
                    Observation = observationNotNull,
                    TaxonId = taxonId
                });
                await _dbContext.SaveChangesAsync();
                return res.Entity;
            }
            else
            {
                throw new ObservationDoesNotExistException(observationId);
            }
        }
        else
        {
            throw new UserDoesNotExistException(authorId);
        }
    }
}
