namespace AI.StarterApp.TicketTriage;

internal static class InVisibleDataSet
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

        var rapid_blinking = AddFaq("What should I do if the light is blinking rapidly?",
            "Open the app and search for recalibration. Follow the instructions and place your phone on the sensor's NFC symbol when prompted.");

        var data_export = AddFaq("Can I export my measurement data?",
            "Yes. In the app, open the sensor's history, tap the menu and choose Export. You can download the data as a CSV file for the last 48 hours. For longer periods, we refer to your home automation system that you connected the sensors to.");

        var notifications = AddFaq("Can the sensors send me notifications or alerts?",
            "The sensors themselves do not send notifications. Set up alerts, e.g. as SMS, email or push messages, in the home automation system that you connected the sensors to.");

        var home_automation = AddFaq("Which home automation systems are supported?",
            "The sensors work with Home Assistant, openHAB, Apple Home, Google Home, Amazon Alexa and Samsung SmartThings.");

        var battery_type = AddFaq("What kind of battery do the sensors use?",
            "The sensors use standard AAA batteries.");

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

        var allTemperaturesTooHigh = Add("Hi, I have 50 temperature sensors in the office building, have been working fine for a while. Since yesterday, all of them are showing 99 degrees, and I'm talking Celsius here! Need help.",
            new NewTicket(Category.Accuracy, Kind.Critical, [AffectedDevices.TemperatureSensor]));

        Add("Hi, one sensor is broken. Really guys!? This is really critical for me! I need the entire batch to work! Please help me as soon as possible. It's urgent.",
            new NewTicket(Category.Other, Kind.Fault, []));

        var noMovement = Add("Hi, I have a movement sensor that is not working. It is not detecting any movement at all even at the highest sensitivity setting. I have tried changing the battery and it still doesn't work. What can I do?",
            new NewTicket(Category.Connectivity, Kind.Fault, [AffectedDevices.MovementSensor]));

        Add("Hi, I just installed a new movement sensor and it is not detecting my cat. It works for me but not for my cat? Can I adjust it?",
            new FAQ([movementSensitivity]));

        Add("Hello guys. One of my movement sensors suddenly stopped working. I tried a new battery, all sensitivity settings, nothing. What can I do?",
            new Duplicate(noMovement));

        Add("Hi, my temperature sensor is blinking rapidly. What does that mean?",
            new FAQ([rapid_blinking]));

        Add("Dear support. My sensor requires a battery change every 2 to 3 month. Is that normal?",
            new FAQ([battery_life]));

        Add("Dear Support. One of my sensors has a rapid blinking light. It's a temprature sensor. What does that mean?",
            new FAQ([rapid_blinking]));


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

        Add("My sensor's light is blinking rapidly. How do I recalibrate it?",
            new FAQ([rapid_blinking]));

        Add("The LED on my temperature sensor is flashing fast. How can I fix it?",
            new FAQ([rapid_blinking]));

        Add("My humidity sensor's readings keep dropping to zero for a few minutes, then return to normal. The other sensors are unaffected.",
            new NewTicket(Category.Accuracy, Kind.MinorFault, [AffectedDevices.HumiditySensor]));

        Add("I want to know whether the movement sensor can be mounted on a ceiling, or if it only works on walls.",
            new NewTicket(Category.Accuracy, Kind.Question, [AffectedDevices.MovementSensor]));

        Add("My temperature sensor keeps disconnecting from the app, even after I replaced its battery and paired it again.",
            new NewTicket(Category.Connectivity, Kind.Fault, [AffectedDevices.TemperatureSensor]));

        Add("My movement sensor keeps sending motion alerts when the room is empty. I checked the sensitivity and nothing is moving nearby.",
            new Duplicate(unexplainableMovement));

        Add("I'm getting movement notifications from an empty room, even with the sensor set to its lowest sensitivity.",
            new Duplicate(unexplainableMovement));

        Add("Since this morning, all twelve of our movement sensors have been sending alerts continuously in empty rooms. They were working normally yesterday.",
            new NewTicket(Category.Accuracy, Kind.Critical, [AffectedDevices.MovementSensor]));

        Add("My movement sensor detects activity whenever I turn on the TV. Is there a way to prevent that?",
            new FAQ([movement_placement]));

        Add("I run a small sauna studio and want to monitor temperature and humidity inside the sauna cabin, which gets up to 100°C. Will your sensors survive that, and are they certified for it?",
            new NewTicket(Category.Other, Kind.Question, [AffectedDevices.TemperatureSensor, AffectedDevices.HumiditySensor]));

        Add("My cat knocked the temperature sensor off the shelf and now the lid won't close properly. Do you sell replacement lids, or do I need a whole new sensor?",
            new NewTicket(Category.Mounting, Kind.MinorFault, [AffectedDevices.TemperatureSensor]));

        Add("Our auditor wants last quarter's temperature log as a spreadsheet. Can I get the readings out of the app somehow?",
            new FAQ([data_export]));

        Add("I'd like to download yesterday's humidity readings as a CSV to play with in Excel. Is that possible?",
            new FAQ([data_export]));

        Add("I tried to export the last day of readings from my movement sensor as described in the FAQ, but the app shows an error and the CSV file is always empty. Other features of the app work fine.",
            new NewTicket(Category.Other, Kind.Fault, [AffectedDevices.MovementSensor]));

        var stuckHumidity = Add("Our greenhouse sensors are great, but last night a thunderstorm hit and now the humidity sensor on the north wall only reports 100% no matter what. The temperature sensor right next to it is fine.",
            new NewTicket(Category.Accuracy, Kind.MinorFault, [AffectedDevices.HumiditySensor]));

        Add("After the storm yesterday, one of my humidity sensors shows a flat 100% all the time. Everything else in shed works normally.",
            new Duplicate(stuckHumidity));

        Add("My hallway movement sensor never triggers anymore, not even when I wave right in front of it at maximum sensitivity. A fresh battery didn't help either.",
            new Duplicate(noMovement));

        Add("I'm running a wine cellar and need a temperature alert when it gets above 16°C. Can your sensors send me an SMS or email?",
            new FAQ([notifications]));

        Add("Does your humidity sensor work with Home Assistant? I'd like to add it to my setup.",
            new FAQ([home_automation]));

        Add("I want to get an email when the movement sensor in my garage triggers. Which home automation systems can I connect it to, and who sends the email?",
            new FAQ([notifications, home_automation]));

        Add("I connected my sensors to Home Assistant, but since the update last week the temperature sensor shows as 'unavailable' there, while the app still shows live readings. The humidity sensor is fine.",
            new NewTicket(Category.Connectivity, Kind.MinorFault, [AffectedDevices.TemperatureSensor]));

        Add("Can I use the sensors with my own Raspberry Pi script, without any of the listed home automation systems?",
            new NewTicket(Category.Other, Kind.Question, []));

        Add("We store vaccines in four cold rooms and since 2 a.m. all 24 of our temperature sensors have stopped reporting. Nothing shows up in the app or in our home automation system. We need this fixed immediately, the cold chain is at risk!",
            new NewTicket(Category.Connectivity, Kind.Critical, [AffectedDevices.TemperatureSensor]));

        Add("All 40 movement sensors in our warehouse died within the same week, although we installed fresh batteries a month ago. The alarm system relies on them, so the building is currently unprotected.",
            new NewTicket(Category.Power, Kind.Critical, [AffectedDevices.MovementSensor]));

        Add("URGENT!!! EMERGENCY!!! My temperature sensor in the guest bathroom is showing 0.5 degrees less than my old thermometer. This is a DISASTER and needs to be fixed TODAY or I will tell everyone!",
            new NewTicket(Category.Accuracy, Kind.MinorFault, [AffectedDevices.TemperatureSensor]));

        Add("CRITICAL ISSUE: the lid of my humidity sensor squeaks a little when I open it. I need an answer within the hour, this is completely unacceptable for a product at this price!!!",
            new NewTicket(Category.Other, Kind.MinorFault, [AffectedDevices.HumiditySensor]));

        Add("This is a catastrophe, our whole company depends on this! Why does my one movement sensor in the break room report a motion event every few days at night? I'm sure it's just the cleaning crew but I demand a fix immediately!",
            new NewTicket(Category.Accuracy, Kind.MinorFault, [AffectedDevices.MovementSensor]));

        Add("I'm the facility manager of the office building and our dashboard shows 99 degrees on every single temperature sensor since yesterday. Nobody is sweating, so something is off. Please advise.",
            new Duplicate(allTemperaturesTooHigh));

        Add("Which battery type do I need for the humidity sensor, and how often do I have to swap it? I want to stock up before it runs out.",
            new FAQ([battery_life, battery_type]));

        return new EvaluationSet([.. records], [.. faqArticles]);
    }
}
