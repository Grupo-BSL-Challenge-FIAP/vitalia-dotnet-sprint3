namespace Vitalia.API.Hateoas;

public class HateoasResource<T>
{
    public T Data { get; set; } = default!;

    public IEnumerable<HateoasLink> Links { get; set; } = [];
}