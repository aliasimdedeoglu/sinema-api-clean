namespace CinemaSystem.Domain.Common;
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    void Delete();
}