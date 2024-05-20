namespace SFMLConsoleEmulator

open SFML.Graphics
open SFML.Window

type PollableRenderWindow = 
    inherit RenderWindow
    new : unit -> PollableRenderWindow
    member this.PollAllEvents : Event list =
        let mutable elist: Event list = []
        let mutable event = new Event()
        while this.PollEvent( &event ) do
            elist <-event::elist
        elist