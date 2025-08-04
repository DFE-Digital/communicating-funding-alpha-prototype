using System.Text.Json;
using CommunicationsAlpha2025.Data;
using CommunicationsAlpha2025.Versions.V2.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsAlpha2025.Versions.V2.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
[Produces("application/json")]
public class StatementController(IStaticDataProvider staticDataProvider) : ControllerBase
{
    /// <summary>
    /// Provides statements for a given funding stream ID.
    /// </summary>
    [HttpGet("{fundingStreamId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<Statement> GetStatementById(string fundingStreamId)
    {
        JsonElement publishedProviderFundingStream
            = staticDataProvider.GetPublishedProviderFundingStreamById(fundingStreamId);
        Statement statement = Statement.FromCfsDataDocument(publishedProviderFundingStream);
        return Ok(statement);
    }

    /// <summary>
    /// Provides all statements for all funding streams.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<Statement[]> GetAllStatements()
    {
        IEnumerable<JsonElement> publishedProviderFundingStreams
            = staticDataProvider.GetAllPublishedProviderFundingStreams();
        List<Statement> statements = [];

        foreach (JsonElement fundingStream in publishedProviderFundingStreams)
        {
            try
            {
                Statement statement = Statement.FromCfsDataDocument(fundingStream);
                statements.Add(statement);
            }
            // If any fail, don't fail the whole endpoint. Just log the error and continue.
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing funding stream: {ex.Message}");
            }
        }

        return Ok(statements);
    }
}