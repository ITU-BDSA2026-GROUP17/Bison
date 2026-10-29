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
        userDTO.Email.Should().Be(user.Email);
        userDTO.Id.Should().Be(user.Id);
        user.Should().BeEquivalentTo(userBase);
    }

    [Fact]
    public async Task UserDTOToUserAsyncUserDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var userDTO = new UserDTO(123, "test", "test@email");

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
        user.Email.Should().Be(userDTO.Email);
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
        obs.Taxon.VernacularName.Should().Be(obsDTO.TaxonName);
        obs.Text.Should().Be(obsDTO.Description);
    }

    [Fact]
    public async Task ObservationDTOToObservationAsyncObservationDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        var obsDTO = new ObservationDTO(123, "time", new UserDTO(123, "name", "email"), "taxonName", "desc");

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
        obs.Taxon.VernacularName.Should().Be(obsDTO.TaxonName);
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
        var pplDTO = pplDTOList[0];

        //Act
        var ppl = await db.ToProposalAsync(pplDTO);

        //Assert
        pplDTO.Should().NotBeNull();
        ppl.Should().NotBeNull();
        ppl.Id.Should().Be(pplDTO.Id);
        ppl.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT).Should().Be(pplDTO.CreatedAt);
        ppl.Observation.Id.Should().Be(pplDTO.ObservationId);
        IUserService.ToUserDTO(ppl.Author).Should().Be(pplDTO.Author);
        ppl.Taxon.VernacularName.Should().Be(pplDTO.TaxonName);
        ppl.Text.Should().Be(pplDTO.Text);
    }

    [Fact]
    public async Task ProposalDTOToProposalAsyncProposalDoesNotExistTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        var pplDTO = new ProposalDTO(123, "time", 0, new UserDTO(123, "name", "email"), "taxonName", "desc");

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
        ppl.Taxon.VernacularName.Should().Be(pplDTO.TaxonName);
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
        var cmtDTO = cmtDTOList[0];

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

        var cmtDTO = new CommentDTO(123, "time", 0, new UserDTO(123, "name", "email"), "desc");

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
}
