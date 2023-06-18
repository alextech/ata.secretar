// https://blog.jeremylikness.com/5-rest-api-designs-in-dot-net-core-2-ad2f204c2d11

namespace SharedKernel.Hateoas
{
    public class Link
    {
        public string Href { get; set; }
        public string Method { get; set; }
    }
}
