using Bison.Database.Services;
using Bison.Models;

namespace Bison.Database.Tests;

public class PostServiceIntegrationTests
{
    [Property]
    public static async Task EmptyDbGetPostByIdTest(int id)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new PostService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var post = await db.GetPostAsync(id);

        //Assert
        post.Should().BeNull();
    }

    [Fact]
    public static async Task CorrectDataIsGivenForSeededDatabase()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new PostService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var obsDTO = await db.GetPostAsync(3);
        var cmtDTO = await db.GetPostAsync(13);
        var pplDTO = await db.GetPostAsync(24);

        var obs = await db.ToPostAsync(obsDTO);
        var cmt = await db.ToPostAsync(cmtDTO);
        var ppl = await db.ToPostAsync(pplDTO);

        //Assert
        obsDTO.Should().NotBeNull();
        obsDTO.Observation.Should().NotBeNull();
        obsDTO.Comment.Should().BeNull();
        obsDTO.Proposal.Should().BeNull();
        obsDTO.Id.Should().Be(3);
        obsDTO.Type.Should().Be("Observation");
        obsDTO.Author.Username.Should().Be("WendellBallan");
        obsDTO.Text.Should().Be("Large white heron in a wet meadow. Yellow bill, black legs.");
        obsDTO.Observation.Taxon.DwCId.Should().Be("MSTSNM:Arter:c18811f4-f785-ea11-aa77-501ac539d1ea");
        obs.Id.Should().Be(obsDTO.Id);
        obs.Should().BeOfType<Observation>();
        obs.Author.UserName.Should().Be(obsDTO.Author.Username);
        obs.Text.Should().Be(obsDTO.Text);

        cmtDTO.Should().NotBeNull();
        cmtDTO.Observation.Should().BeNull();
        cmtDTO.Comment.Should().NotBeNull();
        cmtDTO.Proposal.Should().BeNull();
        cmtDTO.Id.Should().Be(13);
        cmtDTO.Type.Should().Be("Comment");
        cmtDTO.Author.Name.Should().Be("Luanna Muro");
        cmtDTO.Text.Should().Be("None that I could read, sorry.");
        cmtDTO.Comment.ObservationId.Should().Be(2);
        cmt.Id.Should().Be(cmtDTO.Id);
        cmt.Should().BeOfType<Comment>();
        cmt.Author.UserName.Should().Be(cmtDTO.Author.Username);
        cmt.Text.Should().Be(cmtDTO.Text);

        pplDTO.Should().NotBeNull();
        pplDTO.Observation.Should().BeNull();
        pplDTO.Comment.Should().BeNull();
        pplDTO.Proposal.Should().NotBeNull();
        pplDTO.Id.Should().Be(24);
        pplDTO.Type.Should().Be("Proposal");
        pplDTO.Author.Username.Should().Be("MellieYost");
        pplDTO.Text.Should().Be("In a harbour it may also be a shag. Look for the crest.");
        pplDTO.Proposal.Taxon.DwCId.Should().Be("MSTSNM:Arter:c4f52bf9-f785-ea11-aa77-501ac539d1ea");
        pplDTO.Proposal.ObservationId.Should().Be(10);
        ppl.Id.Should().Be(pplDTO.Id);
        ppl.Should().BeOfType<Proposal>();
        ppl.Author.UserName.Should().Be(pplDTO.Author.Username);
        ppl.Text.Should().Be(pplDTO.Text);
    }
}
