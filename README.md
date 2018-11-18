#### Introduction
This is a first-person shooter for the blind and visually impaired (called an audiogame) I wrote in 2004 using Visual BASIC.NET. The game uses audio-only cues to represent enemies and other objects. There are no graphics in this game.

#### Support The Project
This project is open source, so it'll remain free. If you'd like to show your support for it, [please consider donating!](http://paypal.me/munawarb)

#### Background
In Treasure Hunt, your goal is to stop a mad scientist from completing his experiment of cloning. For some reason, the US doesn't like the idea of people cloning creatures, so you're the lucky agent sent in to stop this scientist who goes by the name of James Brutus.

You navigate his overly-complex, underground laboratory (which I guess is the basement in his house,) walking through rooms and corridors looking for his cloning machine.

When I first wrote Treasure Hunt, I wanted to make weapons OSP, but decided against it because "boom" sounds are cool. You start out with a gun (I don't know what type, it's just a gun,) a sword to hack your enemy to death, and some poison-tip needles (what?)

On-site weapons (this is where Treasure Hunt starts to get cool) are
- A laser gun (so I guess you can kill your enemies with a laser?)
- A bullet reflector (it's not a weapon, but serves as good armor. I don't have a sarcastic statement here.)
- A remote-controlled missile that you can use to literally blast your way through the game (it's my personal favorite!) Really though, it does have its uses in the mission.
- A Mental Capacity Breacher that you can use to mind-control a guard. I enjoy controlling them and having them kill a bunch of guards. :)

This game is the prequel to Three-D Velocity, which is also available on my GitHub.

#### Why is the code such a mess?
When I had decided to open-source Three-D Velocity, there was a lot of interest around Treasure Hunt. Unfortunately, I had lost the source code to Treasure Hunt years ago (keep in mind that this is a 2004 project.) So, much to my dismay, I couldn't release something that removed the demo restrictions and call it a day.

My next thought was to reverse-engineer the Treasure Hunt executable (a .NET assembly.) Maybe I could modify the IL code. It's not illegal if the original game developer does it, right? So I tried to open th.exe in ILDasm, and it failed!

Sadly, I had protected Treasure Hunt from reverse-engineering so long ago I don't even remember doing it. I had used a tool called .NET Reactor which is one of the more popular tools to make a .NET assembly tamper-resistant.

So, my first goal was to decompile the .NET Reactor protected code so that at least ILDasm could read it. Maybe then I could patch the IL code, and use another tool that Microsoft has provided called Ilasm to produce an executable from IL code.

I found a tool called [De4dot](https://github.com/0xd4d/de4dot) that did exactly that. You give it your .NET Reactor protected assembly, and it unprotects it for you. I celebrated when I was able to take the resulting th.exe and load it into ILDasm. I never thought I'd be the one reverse-engineering the code I had paid so much to be resistant against reverse-engineering, but it ultimately does show that spending wads of cash to protect .NET assemblies is a bad investment and a falsehood being sold to people like me who bought into it all those years ago.

Next, I dumped the IL code. I think if I had looked at the IL code all those years ago in 2004 I'd have been able to patch it, but alas, this was over fourteen years later, and I couldn't find it in me to follow the IL dump from the horrible code I was writing back then.

So, on to my next goal! I wanted a tool to take my IL code mess thing, and give me a pretty Visual Studio project out of it. In comes [.NET Reflector](https://www.red-gate.com/products/dotnet-development/reflector/index) which does exactly that! Amazing!

Using De4dot and .NET Reflector together I was able to recover the source code to a classic. But it wasn't over yet!

If you've reverse-engineered anything in the past, you know that the resulting dump isn't exactly, well, pretty. .NET Reflector took the IL instructions and translated them into VB (the original language Treasure Hunt was written in.) So some things weren't working right, and my horrible code from 2004 became more horrible in O(1) time after it got imported to a Visual Studio project.

From the time I first recovered my source code to the GitHub push, it's taken at least three months to get things in order. Most of that time was me running away in fear of what I was looking at. But once I got my wits together, I started to fill in the missing pieces of the map that weren't there. There were litterally chunks of the map missing, as if either .NET Reflector or De4dot were like "well, we don't like this room, so we'll just wipe it out." Or maybe it was testament to how poorly I had coded Treasure Hunt in 2004 to where even ILDasm fell over when giving an IL dump.

So, if you poke through the source code, keep in mind that most of the code you see is directly translated from IL code.

I had originally tried DotPeek to get a Visual Studio project, but the tool wasn't accessible. Fortunately, .NET Reflector is built using standard Windows GUI components so it worked well with my assistive technology, and their trial version is fully functional for a limited time.

#### What's New
The latest version is 4.44, released on 11/18/2018. If you want to check out the list of changes, you can [view the change log here](changelog.md).

#### Downloading Treasure Hunt
You can either [download the zip file of the master branch](https://github.com/munawarb/Treasure-Hunt/archive/master.zip), or if you're normal like the rest of us and prefer to use Git, just execute:
`git clone https://github.com/munawarb/Treasure-Hunt.git`

Run the file Treasure-Hunt/Bin/Debug/th.exe to start the game. If you find any issues or have suggestions, simply post them on the issues page. And most of all, have fun! It's been quite a challenge for me to get this game working from nothing but a Reactor-protected executable, so I really hope you enjoy this classic. Oh yeah, I removed the demo restrictions. You're welcome.