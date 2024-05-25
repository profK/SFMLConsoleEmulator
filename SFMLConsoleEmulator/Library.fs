namespace SFMLConsoleEmulator

open SFML.Graphics
open SFML.System
open SFML.Window


module public Console =
    let mutable window = new PollableRenderWindow(VideoMode(800u, 600u), "Console")
    let mutable cursorPosition = new Vector2u(0u, 0u)
    let font = new Font("Courier Prime.ttf")
    let texttop = (new Text("W", font, 12u)).GetLocalBounds().Top
    
    let textSize =
        let wtxt = new Text("W", font, 12u)
        let bounds = wtxt.GetLocalBounds()
        (uint32 (bounds.Width), uint32 (bounds.Height)+(uint32)wtxt.LineSpacing)
      // this is a bit ugly but is here to preserve the behavior of the original Console API
    // in F# these should really be functions so they recalculate when called
    let mutable WindowWidth:int =
        int (window.Size.X / fst textSize)
    let mutable WindowHeight:int =
        int (window.Size.Y / snd textSize)    
    let SetPixelSize (width, height) =
        window.Size <- new Vector2u(width, height)
        WindowHeight <- int (window.Size.Y / snd textSize)
        WindowWidth <-  int (window.Size.X / fst textSize)
    let SetWindowSize (columns:int, rows:int) =
        let width = columns * int (fst textSize)
        let height = rows * int (snd textSize)
        SetPixelSize (uint32 width, uint32 height)
        
    let SetCursorPosition (x:int, y:int) =
        cursorPosition <- Vector2u(uint32 x, uint32 y)
        
    let Write (text:string) =
        let txt = new Text(text, font, 12u)
        let x=float32 (cursorPosition.X * (fst textSize))
        let y = float32 ((cursorPosition.Y * (snd textSize)) )
        txt.Position <- Vector2f(x,y)
        txt.Origin <- Vector2f(0.0f, 0.0f)
        window.Draw(txt)
        //cursorPosition <- Vector2u(cursorPosition.X + uint32 text.Length, cursorPosition.Y)
   
    let Clear () =
        window.Clear(Color.Black)
        
     // The following functions are not part of the original Console API
    // but are necessary for the Console Emulator
      // Display must be called at the end of the frame in order to see the results
    let Display() =    
        window.Display()
      
    let mutable keybuffer = []
    let UpdateEvents() : unit =
        window.PollAllEvents()
        |> Seq.iter (fun (evt:Event) ->
            match evt.Type with
            | EventType.Closed -> window.Close()
            | EventType.TextEntered  -> keybuffer <-(uint32)evt.Key.Code :: keybuffer
            | EventType.KeyReleased  -> keybuffer <-(uint32)evt.Key.Code :: keybuffer
            | EventType.Resized ->
            // update the view to the new size of the window
                let view = new View(FloatRect(0.0f, 0.0f, float32 evt.Size.Width, float32 evt.Size.Height))
                window.SetView view
            | _ -> ()
           ) 
        
        
    let KeyAvailable() =
        UpdateEvents()
        if keybuffer.Length > 0 then true else false
        
    //This currently spin locks, but it could and probably should  be changed to use a wait handle
    let ReadKey() =
        while not (KeyAvailable()) do
            UpdateEvents()
        let key = keybuffer.Head
        keybuffer <- keybuffer.Tail
        key

            
        
        

