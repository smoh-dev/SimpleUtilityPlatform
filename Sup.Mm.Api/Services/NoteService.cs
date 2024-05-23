using Sup.Mm.Api.Repositories;
using Sup.Mm.Common.DTO;

namespace Sup.Mm.Api.Services;

public class NoteService
{
    private readonly IDbRepository _db;

    public NoteService(IDbRepository db)
    {
        _db = db;
    }

    public async Task<List<NoteTagDto>> GetNotesAsync()
    {
        var result = await _db.GetNoteTagAsync<NoteTagDto>();
        return result;
    }
}