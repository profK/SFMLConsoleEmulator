module FSSnake.ConsoleEmulator

open SFML.Graphics
open SFML.System
open SFML.Window


module Console =
    let mutable window = new RenderWindow(VideoMode(800u, 600u), "Console")
    let font = new Font("Courier Prime.ttf")
    let SetPixelSize (width, height) =
        window.Size <- new Vector2u(width, height)
    let SetSize (columns, rows) =
        let wtxt = new Text("W", font, 12u)
        let charsz = (wtxt.LetterSpacing,wtxt.LineSpacing)
        let width = columns * (fst charsz)
        let height = rows * (snd charsz)
        SetPixelSize (uint32 width, uint32 height)
        
        

