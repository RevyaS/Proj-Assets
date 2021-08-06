# Proj-Assets 
Project Assets, a CYOA, simulator or generic game that can be used to tell a story

Current Version: .05

[Project documentations and Assets collection link](https://drive.google.com/drive/folders/1svMfYqJbfHz7dFtQRj-TgUTQsn_5sggx)

**Change notes:** 
>You know what? Fuck it, I'll just add features while working on the story, I don't need to complete the story for the entire version, I'm a discovery writer afterall
>.05 is now done, I'll update the next changes while writing to see which parts I feel are missing

**Planned changes completed  for v0.1:**
- [ ] Default Music for every story becuz I'm too lazy to write the "BGM" key on every Location registered
- [ ] Transition System like an array of images to be registered in StoryData.id files, or probably a registered animation in the AnimationPlayer Node I already attached inside TopGUI Node, I can't decide which is better
- [ ] Animation System, similar to Transition System but I think it's better fit to utilize an array of images instead
- [ ] Conditional Events like there are 2 "Event2" keys wherein 1 will override the default "Event2" key if being triggered
- [ ] Default Events, read Conditional Events for more details
- [ ] A new main menu GUI that shows the Plot of the story along with it's cover image while hovered by mouse
- [ ] Cover image for every story, read above description of new main menu GUI for more details
- [ ] Faster Save file reading, I think there's like a 0.4s delay when opening the Data Selection page for the 1st time
- [ ] Hidden Stats, unlike Chars stats in StoryData.id that is update based, I'm thinking of a HiddenStats key that is incremental instead of updateable

More features to be added later, I'm considering switching the format of .id files to YAML though I'm scared of the potential file reading speed drop
