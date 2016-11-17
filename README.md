# Mixpanel.NET.PCL

A simple portable class library for communicating with Mixpanel

---

### Initialize with a single line

(must have mixpanel token)

```
Client.Initialize("YOUR_MIXPANEL_TOKEN");
```

### Create a Profile

Without a distinct id specified:
```
var p = new Profile();
```
or with one:
```
var p = new Profile("DISTINCT_ID");
```

With either constructor, you can access the identifier with ```p.DistinctIdentifier```

### Set some values for your profile

You can access the full Mixpanel Engage API

```
var p = new Profile();
var properties = new Dictionary<string, object>
{
    { "Name", "Austin C Roos" },
    { "Favorite Mammal", "Elephant" }
};

p.Set(properties);
```

### Track some events

You can create custom event types
```
using Mixpanel.NET.PCL;

namespace AwesomeMixpanelTracker
{
    class CustomMixpanelEvent : IMixpanelEvent
    {
        public string EventName { get; }
        
        public string ImportantNumber { get; set; }
        
        public CustomMixpanelEvent(string name, int random)
        {
            EventName = name;
            ImportantNumber = random;
        }
    }
}
```
You can access the full Mixpanel Track API and track with either a custom event, a dictionary, or simply an event name.  You can choose to associate an event with a profile or not.

```
var t = new EventTracker();
t.Track("Random Event");
var properties = new Dictionary<string, object>
{
    { "Property 1", "Value 1" },
    { "Property 2", 2 }
};
t.Track("Specific event", properties);

var evt = new CustomMixpanelEvent("Custom Event", 1001);
t.Track(evt);
```

You can also set some global properties on your event tracker, which will apply those properties to every event sent from the tracker.  (I find this useful for attaching a version to all events in a given session)

```
var globalProps = new Dictionary<string, object>
{
    { "Version", 0.0.0 }
    { "IP", 192.168.101.101 }
};
var t = new EventTracker(globalProps);
```

You can easily track exceptions by using TrackException

```
var t = new EventTracker();
try
{
    throw new Exception("wee!");
}
catch(Exception e)
{
    t.TrackException(e);
}
```

You can time events.  Start by calling StartTimedTrackingEvent and finish by calling Track (both with the same event name).  This will automatically add a duration property to your event (in seconds).

```
var t = new EventTracker();
t.StartTimedTrackingEvent("Long event");
Thread.Sleep(100000000000000000);
t.Track("Long event");
```
