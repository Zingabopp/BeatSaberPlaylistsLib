# New in v1.2.2
* CustomData support for `bplist` and `blist` playlists.
  * Access with `TryGetCustomData` and `SetCustomData`
* New `PlaylistManager` operations:
  * `RefreshPlaylists`: Repopulates the playlists with data loaded from file.
  * `CreateChildManager`: Creates a new child `PlaylistManager` and an associated directory.
  * `DeleteChildManager`: Deletes a child manager and the associated directory.
  * `RenameManager`: Renames the `PlaylistManager` and the associated directory.
