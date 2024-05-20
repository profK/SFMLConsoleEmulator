namespace SFMLConsoleEmulator

open SFML.Graphics
open SFML.System
open SFML.Window


module public Console =
    let mutable window = new RenderWindow(VideoMode(800u, 600u), "Console")
    let mutable cursorPosition = new Vector2u(0u, 0u)
    let font = new Font("Courier Prime.ttf")
    
    let textSize =
        let wtxt = new Text("W", font, 12u)
        let bounds = wtxt.GetGlobalBounds()
        (uint32 bounds.Width, uint32 bounds.Height)
    let SetPixelSize (width, height) =
        window.Size <- new Vector2u(width, height)
        
    let SetWindowSize (columns:int, rows:int) =
        let width = columns * int (fst textSize)
        let height = rows * int (snd textSize)
        SetPixelSize (uint32 width, uint32 height)
        
    let WindowWidth:int =
        int (window.Size.X / fst textSize)
    let WindowHeight:int =
        int (window.Size.Y / snd textSize)
        
    let SetCursorPosition (x:int, y:int) =
        cursorPosition <- Vector2u(uint32 x, uint32 y)
        
    let Write (text:string) =
        let txt = new Text(text, font, 12u)
        txt.Position <-
            Vector2f(float32 (cursorPosition.X * (fst textSize)), float32 (cursorPosition.Y * snd textSize))
        window.Draw(txt)
        cursorPosition <- Vector2u(cursorPosition.X + uint32 text.Length, cursorPosition.Y)
   
    let Clear () =
        window.Clear(Color.Black)
        
     // The following functions are not part of the original Console API
    // but are necessary for the Console Emulator
      // Display must be called at the end of the frame in order to see the results
    let Display() =    
        window.Display()
        

