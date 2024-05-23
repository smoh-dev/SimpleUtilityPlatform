namespace Sup.Mm.Api.Repositories;

public interface IDbRepository
{
    /// <summary>
    /// Get all note-tag info from database
    /// </summary>
    public Task<List<T>> GetNoteTagAsync<T>();
}