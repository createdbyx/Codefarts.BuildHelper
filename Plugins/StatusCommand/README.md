## Status Plugin

Reports messages to a IStatusReporter implementation.

### Usage

    <status test="Some status" category="SomeCategory" progress="25.62"/>

### Arguments
 
* text - The message to be reported. Default is null.
* type - The message type. Default is message.
* category - The message category. Default is null.
* progress - The progress being reported. Default is zero.

### Notes
* Variables specified inside of text or category arguments will be replaced. 