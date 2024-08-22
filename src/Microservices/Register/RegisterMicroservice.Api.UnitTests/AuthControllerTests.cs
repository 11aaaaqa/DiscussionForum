using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using RegisterMicroservice.Api.Controllers;
using RegisterMicroservice.Api.DTOs.Auth;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;

namespace RegisterMicroservice.Api.UnitTests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk()
        {
            //arrange
            
            var registerModel = new RegisterDto
            {
                Email = "testEmail@testEmail.test",
                Password = "testPassword",
                PasswordConfirm = "testPassword",
                UserName = "testUser",
                Uri = new ConfirmEmailMethodUri
                {
                    Action = "testAction",
                    Controller = "testController",
                    DomainName = "testDomainName",
                    Protocol = "testProtocol"
                }
            };

            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);
            userManagerMock
                .Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var loggerMock = new Mock<ILogger<AuthController>>();

            var tokenServiceMock = new Mock<ITokenService>();

            var emailSenderMock = new Mock<IEmailSender>();
            emailSenderMock.Setup(x => x.SendEmailAsync(
                new MailboxAddress("", registerModel.Email), "test","test"));

            var controller = new AuthController(userManagerMock.Object, tokenServiceMock.Object,loggerMock.Object, emailSenderMock.Object);

            //act

            var result = await controller.Register(registerModel);

            //assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsConflict()
        {
            //arrange
            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError{
                    Code = "",
                    Description = ""
                })));

            var controller = new AuthController(userManagerMock.Object,
                new Mock<ITokenService>().Object,
                new Mock<ILogger<AuthController>>().Object,
                new Mock<IEmailSender>().Object);

            //act
            var result = await controller.Register(new RegisterDto
            {
                Email = "test@test.test",
                Password = "testPsw",
                PasswordConfirm = "testPsw",
                UserName = "testUN",
                Uri = new ConfirmEmailMethodUri
                {
                    Action = "testAct",
                    Controller = "testController",
                    DomainName = "testDomain",
                    Protocol = "testProtocol"
                }
            });

            //assert

            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}
