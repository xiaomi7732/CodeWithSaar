namespace DI.ServiceContainerBasics
{
    public class DogReport
    {
        public void Print(Dog dog, ISerializer serializer, IOutputter outputter)
        {
            string dogJson = serializer.SerializeObject(dog);
            outputter.WriteLine(dogJson);
        }
    }
}