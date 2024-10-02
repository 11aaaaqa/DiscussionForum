using Microsoft.AspNetCore.Mvc;
using TopicMicroservice.Api.DTOs;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IRepository<Topic> topicRepository;
        private readonly ILogger<TopicController> logger;

        public TopicController(IRepository<Topic> topicRepository, ILogger<TopicController> logger)
        {
            this.topicRepository = topicRepository;
            this.logger = logger;
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAllTopicsAsync() => Ok(await topicRepository.GetAllAsync());

        [Route("GetByName")]
        [HttpGet]
        public async Task<IActionResult> GetTopicByNameAsync(string name)
        {
            var topic = await topicRepository.GetByNameAsync(name);
            if (topic == null)
            {
                logger.LogError("Wrong topic name, end method");
                return BadRequest("Такой темы не существует");
            }
            return Ok(topic);
        }

        [Route("GetById")]
        [HttpGet]
        public async Task<IActionResult> GetTopicByIdAsync(Guid id)
        {
            var topic = await topicRepository.GetByIdAsync(id);
            if (topic == null)
            {
                logger.LogError("Wrong topic id, end method");
                return BadRequest("Такой темы не существует");
            }
            return Ok(topic);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopicAsync([FromBody]TopicDto model)
        {
            var topic = await topicRepository.GetByNameAsync(model.Name);
            if (topic != null)
            {
                logger.LogError("Topic wasn't created because it is already exists");
                return BadRequest("Такая тема уже существует");
            }

            var result = await topicRepository.CreateAsync(new Topic
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                PostsCount = 0
            });

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTopicAsync([FromBody] Topic model)
        {
            var topic = await topicRepository.GetByIdAsync(model.Id);
            if (topic == null)
            {
                logger.LogError("Topic wasn't updated because it doesn't exist");
                return BadRequest("Темы с таким идентификатором не существует");
            }

            var result = await topicRepository.UpdateAsync(model);
            return Ok(result);
        }

        [HttpDelete("DeleteById")]
        public async Task<IActionResult> DeleteTopicByIdAsync(Guid id)
        {
            var topic = await topicRepository.GetByIdAsync(id);
            if (topic != null)
            {
                await topicRepository.DeleteByIdAsync(id);
                return Ok();
            }
            logger.LogError("Topic wasn't deleted because current topic id doesn't exist");
            return BadRequest("Темы с таким идентификатором не существует");
        }

        [HttpDelete("DeleteByName")]
        public async Task<IActionResult> DeleteTopicByNameAsync(string name)
        {
            var topic = await topicRepository.GetByNameAsync(name);
            if (topic != null)
            {
                await topicRepository.DeleteByNameAsync(name);
                return Ok();
            }
            logger.LogError("Topic wasn't deleted because current topic name doesn't exist");
            return BadRequest("Темы с таким названием не существует");
        }
    }
}
