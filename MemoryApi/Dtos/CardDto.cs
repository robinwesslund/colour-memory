using MemoryApi.Domain;

namespace MemoryApi.Dtos;

public sealed record CardDto(int Index, CardState State, string? Symbol);
