# .Net core support for Inno Setup

This is a proof of concept to use a .Net dll as an extension to Inno Setup.

The .Net dll is basically a COM host, but it doesn't need registration, because we load it directly from the Inno setup script.

The nice thing about COM is that everything just works. If you put the reference to the COM-object in a Variant, you can access all properties and methods. Also exceptions are nicely carried over to Inno.

The supplied extension (in mysetup.exe) is compiled against .Net 6.



**Note 1**: CreateComObject doesn't work for COM objects that are generated from .Net6 or higher. That is mainly caused by not having a typelib exposed. But, creating an IDispatch and put that into a Variant circumvents that limitation. 

**Note 2**: The .Net desktop runtime(x86) is needed before you can use the extension.
