Console.WriteLine("Begin Process");
for (var i = 0; i <= 100; i++) {
    Console.WriteLine($"Howdy {i}");
    System.Threading.Thread.Sleep(1000);
}
Console.WriteLine("All Done");

