# DotStack
A sample tool to quickly print out managed stacks for dotnet applications
```
usage: DotStack <pid>
```

## Publish a single file
The project is setup to use CoreRT to publish a single file that has been AOT compiled.  The final binary is roughly 21 MB.
```
$ dotnet publish -c release -r <RID> -o ./native   # uses CoreRT to AOT compile the app
$ strip ./native/DotStack                          # strips the debug information from the binary, reducing the file size
```