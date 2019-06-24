namespace HttpService
{
    interface ICrudController
    {
        byte[] Delete();
        byte[] Get();
        byte[] Post();
        byte[] Put();
    }
}