# synth
Digital audio synthesis.

This is a repository of my exploration of digital audio synthesis. No pre-recorded samples, everything derived mathematically.  This is not a production quality solution, nor is it intended to be so. There is no commitment to a stable API or ABI.

## Environment
The solution currently targets .Net Framework 5 (VS2019 Update 4) on Windows. Most of the core synthesis parts (ErnstTech.SoundCore) should work cross-platform. The test application (Synthesizer) requires Windows for audio playback. It is otherwise a basic WinForms application.

## Dependencies
The only external dependency other than the .Net Framework is Antlr4.  There is a soft dependency on SharpDX for exploratory work with DirectX playback limited to the XAudioDynSample application. This will likely go away in the future.

## Testing
Test coverage, at this point, is minimal. The only automated testing is around expression parsing and signal generation. The automated tests in ErnstTech.SoundCore.Tests should not require Windows to run.

## History
This repository was a recently resurfaced old project, originally targetting .Net 1.x and managed DirectX. Work on modernizing it is on going, but on an as-needed basis.
