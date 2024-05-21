namespace SFMLConsoleEmulator

open SFML.Graphics
open SFML.Window

type PollableRenderWindow(V:VideoMode,t:string) =
    inherit RenderWindow(V,t)
    new() = new PollableRenderWindow(VideoMode(800u,600u),"")
    member this.PollAllEvents (): Event list =
        let mutable elist: Event list = []
        let mutable event = new Event()
        while this.PollEvent( &event ) do
            elist <-event::elist
        elist