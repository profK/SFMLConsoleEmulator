﻿// This is a .NET implementation of the console game snake.
// The game is written in F# and uses an emulation of the console API to do input and output.
// The game is a simple implementation of the classic snake game where the player controls a snake
// that grows in length as it eats food.
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
    Message: string
}


let mutable gameState = {
    Direction= (0,0)
    Snake= [(Console.WindowWidth/2, Console.WindowHeight/2)]
    Food= (0,0)
    Score= 0
    Collision = false
    Message = ""
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
    Console.SetCursorPosition(0, Console.WindowHeight-2)
    Console.Write("+")
    Console.SetCursorPosition(Console.WindowWidth-1, Console.WindowHeight-3)
    Console.Write("+")
    for i in 1..(Console.WindowWidth-2) do
        Console.SetCursorPosition(i, 0)
        Console.Write("-")
        Console.SetCursorPosition(i, Console.WindowHeight-3)
        Console.Write("-")
    for i in 1..(Console.WindowHeight-3) do
        Console.SetCursorPosition(0, i)
        Console.Write("|")
        Console.SetCursorPosition(Console.WindowWidth-1, i)
        Console.Write("|")

let rec generateFood gstate  =
    let loc = (random.Next(1, Console.WindowWidth-2), random.Next(1, Console.WindowHeight-4))
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
    let newHeadX = (fst gstate.Snake.Head) + (fst gstate.Direction)
    let newHeadY = (snd gstate.Snake.Head) + (snd gstate.Direction)

    let newsnake =
        (newHeadX,newHeadY)::(gstate.Snake[0..(gstate.Snake.Length)-2])
    {gstate with Snake = newsnake}


let  checkCollision gstate =
    if (fst gstate.Snake.Head)=0 || (fst gstate.Snake.Head) = Console.WindowWidth-1 ||
       (snd gstate.Snake.Head) =  0 || (snd gstate.Snake.Head) = Console.WindowHeight-3 then
        {gstate with Collision = true; Message = "Collided with wall" }
    else if List.tail gstate.Snake |> List.contains gstate.Snake.Head then
        {gstate with Collision = true; Message = "Collided with self"}
    else
        gstate
        
let doInput gstate : GameState =
    if Console.KeyAvailable() then
        let key = Console.ReadKey()
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
    Console.SetCursorPosition (0, Console.WindowHeight-2)
    Console.Write ("Score: " + gstate.Score.ToString())
    


[<EntryPoint>]
let main argv =
    // Steup
    Console.SetWindowSize (80,  80)
    Console.CursorVisible <- false
    gameState <- generateFood gameState
    while not gameState.Collision do
       Console.Clear() //clear last frame draws
       drawBorder() // draw the border to the new frame
       gameState <- // calculate the new game state
           gameState |> doInput |> moveSnake |> checkCollision |> tryEat
       drawGameState gameState //draw the new game state to the screen
       Console.Display() //show the new frame
       Thread.Sleep(100) //wait for 1/10th of a second before the next frame
    //Draw the end of game screen   
    let messageLength = gameState.Message.Length
    Console.SetCursorPosition((Console.WindowWidth/2)-(messageLength/2), Console.WindowHeight/2)
    Console.Write(gameState.Message)
    Console.SetCursorPosition((Console.WindowWidth/2)-4, (Console.WindowHeight/2)+2)
    Console.Write("GAME OVER")
    Console.Display()
    Thread.Sleep(10*1000) //display end of game screen for 10 seconds before exiting
    0     
