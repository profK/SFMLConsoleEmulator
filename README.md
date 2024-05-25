This is a little F# library that (mostly) emulates the Windows Console API using SFML.
Big differences are that you need to need to call window.Display once a frame and
that keyboard events return the scancode, not the VK code.

It includes an example implementation of the Snake game in F#
