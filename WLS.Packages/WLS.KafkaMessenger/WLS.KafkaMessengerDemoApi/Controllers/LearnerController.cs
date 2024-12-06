using KafkaMessengerDemoApi.LearnerApi.Models;

using Microsoft.AspNetCore.Mvc;

using WLS.KafkaMessenger;

namespace KafkaMessengerDemoApi.Controllers
{
	[Route("api/v{version:apiVersion}")]
	[ApiController]
	[Consumes("application/json")]
	[ApiVersion("1.0")]
	public class LearnerController : ControllerBase
	{
		private readonly ILearnerService _learnerService;

		public LearnerController(ILearnerService learnerService)
		{
			_learnerService = learnerService;
		}

		[HttpPut]
		[ProducesResponseType(typeof(ReturnValue), 200)]
		[ProducesResponseType(400)]
		public IActionResult SendLearnerUpdatedMessage(LearnerAccount learnerAccount)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			else
				return Ok(_learnerService.SendLearnerUpdatedMessage(learnerAccount));
		}
	}
}
