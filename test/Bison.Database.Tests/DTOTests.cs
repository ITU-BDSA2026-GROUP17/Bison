using Bison.Database.Services;

namespace Bison.Database.Tests;

public class DTOTests
{
    // User Tests
    [Fact]
    public async Task UserDTOToUserAsyncSuccessfulTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var userBase = (await UserServiceIntegrationTests.UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var userId = userBase.Id;

        //Act
        var userDTO = await db.GetUserByIdAsync(userId);
        var user = await db.ToUserAsync(userDTO);

        //Assert
        userDTO.Should().NotBeNull();
        user.Should().NotBeNull();
        userDTO.Name.Should().Be(user.Name);
        userDTO.Id.Should().Be(user.Id);
        user.Should().BeEquivalentTo(userBase);
    }

    [Fact]
    public async Task UserDTOToUserAsyncUserDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var userDTO = new UserDTO("123", "test", "test@example.com");

        //Act
        var user = await db.ToUserAsync(userDTO);

        //Assert
        userDTO.Should().NotBeNull();
        user.Should().BeNull();
    }

    [Fact]
    public async Task UserAsyncToUserDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var user = (await UserServiceIntegrationTests.UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        //Act
        var userDTO = IUserService.ToUserDTO(user);

        //Assert
        userDTO.Should().NotBeNull();
        user.Should().NotBeNull();
        user.Name.Should().Be(userDTO.Name);
        user.Id.Should().Be(userDTO.Id);
    }

    // Observations
    [Fact]
    public async Task ObservationDTOToObservationAsyncSuccessfulTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        var obsDTO = await db.GetObservationByIdAsync(1);

        //Act
        var obs = await db.ToObservationAsync(obsDTO);

        //Assert
        obsDTO.Should().NotBeNull();
        obs.Should().NotBeNull();
        obs.Id.Should().Be(obsDTO.Id);
        obs.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(obsDTO.CreatedAt);
        IUserService.ToUserDTO(obs.Author).Should().Be(obsDTO.Author);
        obs.Taxon.VernacularName.Should().Be(obsDTO.Taxon.VernacularName);
        obs.Text.Should().Be(obsDTO.Description);
    }

    [Fact]
    public async Task ObservationDTOToObservationAsyncObservationDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        var obsDTO = new ObservationDTO(123, "time", new UserDTO("123", "name", "test@example.com"), new TaxonDTO("", "taxonName", null), "desc");

        //Act
        var obs = await db.ToObservationAsync(obsDTO);

        //Assert
        obsDTO.Should().NotBeNull();
        obs.Should().BeNull();
    }

    [Fact]
    public async Task ObservationAsyncToObservationDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);

        var obs = await repo.GetObservation(3);

        //Act
        var obsDTO = IObservationService.ToObservationDTO(obs);

        //Assert
        obsDTO.Should().NotBeNull();
        obs.Should().NotBeNull();
        obs.Id.Should().Be(obsDTO.Id);
        obs.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(obsDTO.CreatedAt);
        IUserService.ToUserDTO(obs.Author).Should().Be(obsDTO.Author);
        obs.Taxon.VernacularName.Should().Be(obsDTO.Taxon.VernacularName);
        obs.Text.Should().Be(obsDTO.Description);
    }

    // Proposals
    [Fact]
    public async Task ProposalDTOToProposalAsyncSuccessfulTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        var pplDTOList = await db.GetProposalsByObservationIdAsync(3);
        var pplDTO = pplDTOList[1]; // Using index 1 to improve branch coverage.

        //Act
        var ppl = await db.ToProposalAsync(pplDTO);

        //Assert
        pplDTO.Should().NotBeNull();
        ppl.Should().NotBeNull();
        ppl.Id.Should().Be(pplDTO.Id);
        ppl.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(pplDTO.CreatedAt);
        ppl.Observation.Id.Should().Be(pplDTO.ObservationId);
        IUserService.ToUserDTO(ppl.Author).Should().Be(pplDTO.Author);
        ppl.Taxon.VernacularName.Should().Be(pplDTO.Taxon.VernacularName);
        ppl.Text.Should().Be(pplDTO.Text);
    }

    [Fact]
    public async Task ProposalDTOToProposalAsyncProposalDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        var pplDTO = new ProposalDTO(123, "time", 0, new UserDTO("123", "name", "test@example.com"), new TaxonDTO("", "taxonName", null), "desc");

        //Act
        var ppl = await db.ToProposalAsync(pplDTO);

        //Assert
        pplDTO.Should().NotBeNull();
        ppl.Should().BeNull();
    }

    [Fact]
    public async Task ProposalAsyncToProposalDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ProposalService(repo);

        var pplList = await repo.GetProposalsForObservation(3);
        var ppl = pplList[0];

        //Act
        var pplDTO = IProposalService.ToProposalDTO(ppl);

        //Assert
        pplDTO.Should().NotBeNull();
        ppl.Should().NotBeNull();
        ppl.Id.Should().Be(pplDTO.Id);
        ppl.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(pplDTO.CreatedAt);
        ppl.Observation.Id.Should().Be(pplDTO.ObservationId);
        IUserService.ToUserDTO(ppl.Author).Should().Be(pplDTO.Author);
        ppl.Taxon.VernacularName.Should().Be(pplDTO.Taxon.VernacularName);
        ppl.Text.Should().Be(pplDTO.Text);
    }

    // Comments
    [Fact]
    public async Task CommentDTOToCommentAsyncSuccessfulTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new CommentService(new DatabaseRepository(dbOptions.BisonDbContext));

        var cmtDTOList = await db.GetCommentsByObservationIdAsync(3);
        var cmtDTO = cmtDTOList[1]; // Using index 1 to improve branch coverage.

        //Act
        var cmt = await db.ToCommentAsync(cmtDTO);

        //Assert
        cmtDTO.Should().NotBeNull();
        cmt.Should().NotBeNull();
        cmt.Id.Should().Be(cmtDTO.Id);
        cmt.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(cmtDTO.CreatedAt);
        cmt.Observation.Id.Should().Be(cmtDTO.ObservationId);
        IUserService.ToUserDTO(cmt.Author).Should().Be(cmtDTO.Author);
        cmt.Text.Should().Be(cmtDTO.Comment);
    }

    [Fact]
    public async Task CommentDTOToCommentAsyncCommentDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new CommentService(new DatabaseRepository(dbOptions.BisonDbContext));

        var cmtDTO = new CommentDTO(123, "time", 0, new UserDTO("123", "name", "test@example.com"), "desc");

        //Act
        var cmt = await db.ToCommentAsync(cmtDTO);

        //Assert
        cmtDTO.Should().NotBeNull();
        cmt.Should().BeNull();
    }

    [Fact]
    public async Task CommentAsyncToCommentDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new CommentService(repo);

        var cmtList = await repo.GetCommentsForObservation(3);
        var cmt = cmtList[0];

        //Act
        var cmtDTO = ICommentService.ToCommentDTO(cmt);

        //Assert
        cmtDTO.Should().NotBeNull();
        cmt.Should().NotBeNull();
        cmt.Id.Should().Be(cmtDTO.Id);
        cmt.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(cmtDTO.CreatedAt);
        cmt.Observation.Id.Should().Be(cmtDTO.ObservationId);
        IUserService.ToUserDTO(cmt.Author).Should().Be(cmtDTO.Author);
        cmt.Text.Should().Be(cmtDTO.Comment);
    }

    // Comments
    [Fact]
    public async Task SubpostDTOToPostAsyncSuccessfulTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        var subPostDTOList = await db.GetSubPostsAsync(3);
        var postDTO = subPostDTOList[1]; // Using index 1 to improve branch coverage.

        //Act
        var post = await db.ToPostAsync(postDTO);

        //Assert
        postDTO.Should().NotBeNull();
        post.Should().NotBeNull();
        post.Id.Should().Be(postDTO.CommentDTO?.Id ?? postDTO.ProposalDTO?.Id);
        post.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(postDTO.CommentDTO?.CreatedAt ?? postDTO.ProposalDTO?.CreatedAt);
        3.Should().Be(postDTO.ObservationId);
        IUserService.ToUserDTO(post.Author).Should().Be(postDTO.CommentDTO?.Author ?? postDTO.ProposalDTO?.Author);
        post.Text.Should().Be(postDTO.CommentDTO?.Comment ?? postDTO.ProposalDTO?.Text);
    }

    [Fact]
    public async Task CommentSubpostDTOToPostAsyncPostDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        var user = new UserDTO("123", "name", "test@example.com");
        var subPostDTO = new SubPostDTO(123,
            new CommentDTO(1234, "", 123, user, ""),
            null
        );

        //Act
        var post = await db.ToPostAsync(subPostDTO);

        //Assert
        subPostDTO.Should().NotBeNull();
        post.Should().BeNull();
    }

    [Fact]
    public async Task ProposalSubpostDTOToPostAsyncPostDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        var user = new UserDTO("123", "name", "test@example.com");
        var subPostDTO = new SubPostDTO(123,
            null,
            new ProposalDTO(1234, "", 123, user, new TaxonDTO("", "", null), "")
        );

        //Act
        var post = await db.ToPostAsync(subPostDTO);

        //Assert
        subPostDTO.Should().NotBeNull();
        post.Should().BeNull();
    }

    [Fact]
    public async Task PostAsyncToSubpostDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);

        var postList = await repo.GetSubpostsForObservation(3);
        var post = postList[0];

        //Act
        var postDTO = IObservationService.ToPostDTO(post);

        //Assert
        postDTO.Should().NotBeNull();
        post.Should().NotBeNull();
        post.Id.Should().Be(postDTO.CommentDTO?.Id ?? postDTO.ProposalDTO?.Id);
        post.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(postDTO.CommentDTO?.CreatedAt ?? postDTO.ProposalDTO?.CreatedAt);
        3.Should().Be(postDTO.ObservationId);
        IUserService.ToUserDTO(post.Author).Should().Be(postDTO.CommentDTO?.Author ?? postDTO.ProposalDTO?.Author);
        post.Text.Should().Be(postDTO.CommentDTO?.Comment ?? postDTO.ProposalDTO?.Text);
    }

    [Fact]
    public async Task InvalidPostAsyncToSubpostDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);

        var post = await repo.GetObservation(3);

        //Act
        try
        {
            IObservationService.ToPostDTO(post);

            //Assert
            Assert.Fail("Should've thrown an exception by now lol");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("post");
            e.Message.Should().Be("Invalid subpost given (Parameter 'post')");
        }
    }

    [Fact]
    public async Task HasBothSubPostDTOAsyncToPostTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(databaseRepository: new DatabaseRepository(dbOptions.BisonDbContext));

        var user = new UserDTO("", "", "");
        var post = new SubPostDTO(
            1,
            new CommentDTO(2, "", 1, user, ""),
            new ProposalDTO(3, "", 1, user, new TaxonDTO("", "", null), "")
        );

        //Act
        try
        {
            await db.ToPostAsync(post);

            //Assert
            Assert.Fail("Should've thrown an exception by now lol");
        }
        catch (ArgumentException e)
        {
            e.Message.Should().Be("SubPostDTO contains either both or none of comment or proposal DTOs");
        }
    }
    [Fact]
    public async Task HasNoneSubPostDTOAsyncToPostTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(databaseRepository: new DatabaseRepository(dbOptions.BisonDbContext));

        var user = new UserDTO("", "", "");
        var post = new SubPostDTO(1, null, null);

        //Act
        try
        {
            await db.ToPostAsync(post);

            //Assert
            Assert.Fail("Should've thrown an exception by now lol");
        }
        catch (ArgumentException e)
        {
            e.Message.Should().Be("SubPostDTO contains either both or none of comment or proposal DTOs");
        }
    }
}
