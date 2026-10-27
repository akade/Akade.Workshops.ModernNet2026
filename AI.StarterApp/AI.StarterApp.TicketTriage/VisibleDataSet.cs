namespace AI.StarterApp.TicketTriage;

internal static class VisibleDataSet
{
    internal static EvaluationSet Data { get; } = Create();

    private static EvaluationSet Create()
    {
        List<FAQArticle> faqArticles = [];

        FAQArticleId AddFaq(string title, string content)
        {
            var id = new FAQArticleId { Id = faqArticles.Count + 1 };
            faqArticles.Add(new FAQArticle(id, title, content));
            return id;
        }

        var temperature_placement = AddFaq("Where should I place my temperature sensor?",
            "Place the temperature sensor in a location that is representative of the area you want to measure. Avoid placing it near heat sources, direct sunlight, or drafts, as these can affect the accuracy of the readings.");

        var movement_placement = AddFaq("Where should I place my movement sensor?",
            "Place the movement sensor in a location that is representative of the area you want to monitor. Avoid placing it near sources of vibration, sources of unrelated movement such as facing towards windows or TVs, as these can affect the accuracy of the readings.");

        var humidity_placement = AddFaq("Where should I place my humidity sensor?",
            "Place the humidity sensor in a location that is representative of the area you want to measure. Avoid placing it near sources of moisture (unless you want measure those), direct sunlight, or heat sources, as these can affect the accuracy of the readings.");

        var battery_life = AddFaq("How long will the battery last?",
            "The battery life of the sensors are as follows: TemperatureSensor: 12 months, MovementSensor: 6 months, HumiditySensor: 12 months.");

        var battery_replacement = AddFaq("How do I replace the battery?",
            "To replace the battery, open the lid of the sensor and remove the old battery. Insert a new battery of the same type, ensuring correct polarity, and close the lid securely. If done within 60 seconds, the sensor will retain its settings.");

        var outdoor = AddFaq("Can I use the sensors outdoors?",
            "All our sensors are waterproof and can be used outdoors");

        var movementSensitivity = AddFaq("How do I adjust the sensitivity of the movement sensor?",
            "You can adjust the sensitivity of the movement sensor by opening the lid, adjust the slider to one of the 7 sensitivity levels.");

        var mounting = AddFaq("How do I mount the sensors?",
            "The sensors can be mounted using the included screws and wall anchors, or you can use double-sided tape for solution without drilling.");

        var setup = AddFaq("How do I set up the sensors?",
            "To set up the sensors, install the battery. The LED will blink slowly. Place your phone with the open app on top of the NFC symbol to proceed.");

        List<EvaluationRecord> records = [];

        Guid Add(string message, TicketAction action)
        {
            var id = Guid.NewGuid();
            records.Add(new EvaluationRecord(new IncomingMessage(message) { Id = id }, action));
            return id;
        }


        Add("Hi, your sensors looks like what I need. Any restriction on where I can use them? In my garden? Thanks",
            new FAQ([temperature_placement, movement_placement, humidity_placement, outdoor]));

        Add("Hello, I hate drilling. Can I mount it differently?",
            new FAQ([mounting]));

        var unexplainableMovement = Add("Hi, my movement sensor is on the lowest sensitivity setting and still triggers randomly. No source of movement nearby, no device or similar. What can I do?",
            new NewTicket(Category.Accuracy, Kind.Fault, [AffectedDevices.MovementSensor]));

        Add("Hi, I have 50 temperature sensors in the office building, have been working fine for a while. Since yesterday, all of them are showing 99 degrees, and I'm talking Celsius here! Need help.",
            new NewTicket(Category.Accuracy, Kind.Critical, [AffectedDevices.TemperatureSensor]));

        Add("Hi, one sensor is broken. Really guys!? This is really critical for me! I need the entire batch to work! Please help me as soon as possible. It's urgent.",
            new NewTicket(Category.Other, Kind.Fault, []));

        var noMovement = Add("Hi, I have a movement sensor that is not working. It is not detecting any movement at all even at the highest sensitivity setting. I have tried changing the battery and it still doesn't work. What can I do?",
            new NewTicket(Category.Connectivity, Kind.Fault, [AffectedDevices.MovementSensor]));

        Add("Hi, I just installed a new movement sensor and it is not detecting my cat. It works for me but not for my cat? Can I adjust it?",
            new FAQ([movementSensitivity]));

        Add("Hello guys. One of my movement sensors suddenly stopped working. I tried a new battery, all sensitivity settings, nothing. What can I do?",
            new Duplicate(noMovement));

        var rapidBlinkingLight = Add("Hi, my temperature sensor is blinking rapidly. What does that mean?",
            new NewTicket(Category.Other, Kind.Fault, [AffectedDevices.TemperatureSensor]));

        Add("Dear support. My sensor requires a battery change every 2 to 3 month. Is that normal?",
            new FAQ([battery_life]));

        Add("Dear Support. One of my sensors has a rapid blinking light. It's a temprature sensor. What does that mean?",
            new Duplicate(rapidBlinkingLight));

        Add("Hello, can I use the sensors outdoors? Also, my employer requires MIL-STD-810G certification. Do you have that?",
            new NewTicket(Category.Other, Kind.Question, []));

        Add("My sensor doesn't do the normal blinking anymore? How do I replace the battery?",
            new FAQ([battery_replacement]));

        Add("My humidity reading is a bit too high. It's just one, the others work properly. What should I do",
            new NewTicket(Category.Accuracy, Kind.MinorFault, [AffectedDevices.HumiditySensor]));

        var shortBatteryLife = Add("My humidity sensor has gone through three batteries in a month, although the FAQ says they should last a year. The other sensors are fine. Is there a fix?",
            new NewTicket(Category.Power, Kind.MinorFault, [AffectedDevices.HumiditySensor]));

        Add("Since this morning, all five of our temperature sensors and three humidity sensors are reporting unusually high readings. They were fine yesterday, and this is affecting our whole site.",
            new NewTicket(Category.Accuracy, Kind.Critical, [AffectedDevices.TemperatureSensor, AffectedDevices.HumiditySensor]));

        Add("I mounted my new sensor. What now?",
            new FAQ([setup]));

        Add("Another humidity sensor is draining batteries in a few weeks. I already reported the same problem with one of these sensors.",
            new Duplicate(shortBatteryLife));

        Add("One temperature sensor is suddenly reading 85°C in an empty room. This is urgent, but all our other sensors are normal.",
            new NewTicket(Category.Accuracy, Kind.MinorFault, [AffectedDevices.TemperatureSensor]));

        Add("I'm following the setup steps, but my phone can't read the NFC tag on the movement sensor, so I can't pair it with the app.",
            new NewTicket(Category.Connectivity, Kind.Fault, [AffectedDevices.MovementSensor]));

        return new EvaluationSet([.. records], [.. faqArticles]);
    }
}
