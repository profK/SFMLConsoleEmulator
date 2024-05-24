// This is a .NET implementation of the console game snake.
// The game is written in F# and uses the .NET Console API for input and output.
// The game is a simple implementation of the classic snake game where the player controls a snake that grows in length as it eats food.
// The player loses if the snake runs into the walls or itself.
// The player can control the snake with the arrow keys.
module FSSnake      
open System
open System.Threading
open SFML.Window
open SFMLConsoleEmulator


let mutable running = true

let random = System.Random()

type GameState = {
    Direction: int * int
    Snake: (int * int) list
    Food: int * int
    Score: int
    Collision: bool
}


let mutable gameState = {
    Direction= (0,0)
    Snake= [(Console.WindowWidth/2, Console.WindowHeight/2)]
    Food= (0,0)
    Score= 0
    Collision = false 
}

//Note: The console emulator returns scan codes, not VK codes like the console API
// so we use the SFML map instead of ConsoleKey
let Controls = [
    uint32 Keyboard.Key.Left, (-1, 0);
    uint32 Keyboard.Key.Right, (1, 0);
    uint32 Keyboard.Key.Up, (0, -1);
    uint32 Keyboard.Key.Down, (0, 1)
]

let drawBorder () =
    Console.SetCursorPosition(0, 0)
    Console.Write("+")
    Console.SetCursorPosition(Console.WindowWidth-1, 0)
    Console.Write("+")
    Console.SetCursorPosition(0, Console.WindowHeight-1)
    Console.Write("+")
    Console.SetCursorPosition(Console.WindowWidth-1, Console.WindowHeight-1)
    Console.Write("+")
    for i in 1..(Console.WindowWidth-2) do
        Console.SetCursorPosition(i, 0)
        Console.Write("-")
        Console.SetCursorPosition(i, Console.WindowHeight-1)
        Console.Write("-")
    for i in 1..(Console.WindowHeight-2) do
        Console.SetCursorPosition(0, i)
        Console.Write("|")
        Console.SetCursorPosition(Console.WindowWidth-1, i)
        Console.Write("|")

let rec generateFood gstate  =
    let loc = (random.Next(1, Console.WindowWidth-2), random.Next(1, Console.WindowHeight-2))
    if List.contains loc gstate.Snake then
        generateFood gstate 
    else
        {gstate with Food=loc}
 
let rec growSnake gstate =
    {gstate with Snake =
                    gstate.Snake @
                    [List.last gstate.Snake
                    |> fun (x, y) -> (x - fst gstate.Direction, y - snd gstate.Direction)]}
let tryEat gstate =
    if gstate.Snake.Head = gstate.Food then
        {gstate with Score=gstate.Score+1}
        |> generateFood
        |> growSnake
    else
        gstate

let moveSnake gstate =
    let newsnake =
        gstate.Snake
        |>List.map (fun (x, y) ->
            (x + (gstate.Direction |> fst) , y + (gstate.Direction |> snd )))
    {gstate with Snake = newsnake}


let  checkCollision gstate =
    if gstate.Snake.Head = (0, 0) || gstate.Snake.Head = (Console.WindowWidth-1, 0) || gstate.Snake.Head = (0, Console.WindowHeight-1) || gstate.Snake.Head = (Console.WindowWidth-1, Console.WindowHeight-1) then
        {gstate with Collision = true}
    else if List.tail gstate.Snake |> List.contains gstate.Snake.Head then
        {gstate with Collision = true}
    else
        gstate
        
let doInput gstate : GameState =
    if Console.KeyAvailable() then
        let key = Console.ReadKey()
        printfn $" keycode: %A{key}" 
        match Controls |> List.tryFind (fun (k, _) -> k = key) with
        | Some (_, dir) -> {gstate with Direction = dir}
        | None -> gstate
    else
        gstate
let drawGameState gstate =
    Console.SetCursorPosition(fst gstate.Food, snd gstate.Food)
    Console.Write("O")
    gstate.Snake
    |> List.iter (fun (x, y) ->
        Console.SetCursorPosition(x, y)
        Console.Write("X") )
    Console.SetCursorPosition (0, Console.WindowHeight)
    Console.Write ("Score: " + gstate.Score.ToString())
    


[<EntryPoint>]
let main argv =
    Console.SetWindowSize(80, 80)
    Console.CursorVisible <- false
    while not gameState.Collision do
       Console.Clear()
       Console.SetCursorPosition(0, 0)
       drawBorder()
       gameState <-
           gameState |> doInput |> moveSnake |> checkCollision |> tryEat
       drawGameState gameState
       Console.Display()
       Thread.Sleep(100)
    0     
