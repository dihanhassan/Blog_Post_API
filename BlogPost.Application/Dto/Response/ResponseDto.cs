namespace BlogPost.Application.Dto.Response
{
    public class ResponseDto<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
