mergeInto(LibraryManager.library, {
  TransmitMessage: function(message) {
    ReactUnityWebGL.TransmitMessage(Pointer_stringify(message));
  }
});