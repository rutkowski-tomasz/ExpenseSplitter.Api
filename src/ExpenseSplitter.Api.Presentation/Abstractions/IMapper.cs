namespace ExpenseSplitter.Api.Presentation.Abstractions;

public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
}
