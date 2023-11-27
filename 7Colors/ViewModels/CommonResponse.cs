namespace _7Colors.ViewModels
{
    public class CommonResponse<T>
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public T? DataEnum { get; set; }

    }
}
