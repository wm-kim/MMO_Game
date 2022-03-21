// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protocol.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Google.Protobuf.Protocol {

  /// <summary>Holder for reflection information generated from Protocol.proto</summary>
  public static partial class ProtocolReflection {

    #region Descriptor
    /// <summary>File descriptor for Protocol.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProtocolReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg5Qcm90b2NvbC5wcm90bxIIUHJvdG9jb2waH2dvb2dsZS9wcm90b2J1Zi90",
            "aW1lc3RhbXAucHJvdG8iMwoLU19FbnRlckdhbWUSJAoGcGxheWVyGAEgASgL",
            "MhQuUHJvdG9jb2wuUGxheWVySW5mbyINCgtTX0xlYXZlR2FtZSIwCgdTX1Nw",
            "YXduEiUKB3BsYXllcnMYASADKAsyFC5Qcm90b2NvbC5QbGF5ZXJJbmZvIh4K",
            "CVNfRGVzcGF3bhIRCglwbGF5ZXJJZHMYASADKAUiJAoGQ19Nb3ZlEgwKBHBv",
            "c1gYASABKAUSDAoEcG9zWRgCIAEoBSI2CgZTX01vdmUSEAoIcGxheWVySWQY",
            "ASABKAUSDAoEcG9zWBgCIAEoBRIMCgRwb3NZGAMgASgFIkgKClBsYXllcklu",
            "Zm8SEAoIcGxheWVySWQYASABKAUSDAoEbmFtZRgCIAEoCRIMCgRwb3NYGAMg",
            "ASgFEgwKBHBvc1kYBCABKAUqXwoFTXNnSWQSEAoMU19FTlRFUl9HQU1FEAAS",
            "EAoMU19MRUFWRV9HQU1FEAESCwoHU19TUEFXThACEg0KCVNfREVTUEFXThAD",
            "EgoKBkNfTU9WRRAEEgoKBlNfTU9WRRAFQhuqAhhHb29nbGUuUHJvdG9idWYu",
            "UHJvdG9jb2xiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.TimestampReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Google.Protobuf.Protocol.MsgId), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.S_EnterGame), global::Google.Protobuf.Protocol.S_EnterGame.Parser, new[]{ "Player" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.S_LeaveGame), global::Google.Protobuf.Protocol.S_LeaveGame.Parser, null, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.S_Spawn), global::Google.Protobuf.Protocol.S_Spawn.Parser, new[]{ "Players" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.S_Despawn), global::Google.Protobuf.Protocol.S_Despawn.Parser, new[]{ "PlayerIds" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.C_Move), global::Google.Protobuf.Protocol.C_Move.Parser, new[]{ "PosX", "PosY" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.S_Move), global::Google.Protobuf.Protocol.S_Move.Parser, new[]{ "PlayerId", "PosX", "PosY" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Protocol.PlayerInfo), global::Google.Protobuf.Protocol.PlayerInfo.Parser, new[]{ "PlayerId", "Name", "PosX", "PosY" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum MsgId {
    /// <summary>
    /// 아래 packet을 naming convention에 따라 맞춰야함, parsing하기 위해서
    /// </summary>
    [pbr::OriginalName("S_ENTER_GAME")] SEnterGame = 0,
    [pbr::OriginalName("S_LEAVE_GAME")] SLeaveGame = 1,
    [pbr::OriginalName("S_SPAWN")] SSpawn = 2,
    [pbr::OriginalName("S_DESPAWN")] SDespawn = 3,
    [pbr::OriginalName("C_MOVE")] CMove = 4,
    [pbr::OriginalName("S_MOVE")] SMove = 5,
  }

  #endregion

  #region Messages
  public sealed partial class S_EnterGame : pb::IMessage<S_EnterGame> {
    private static readonly pb::MessageParser<S_EnterGame> _parser = new pb::MessageParser<S_EnterGame>(() => new S_EnterGame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S_EnterGame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_EnterGame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_EnterGame(S_EnterGame other) : this() {
      player_ = other.player_ != null ? other.player_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_EnterGame Clone() {
      return new S_EnterGame(this);
    }

    /// <summary>Field number for the "player" field.</summary>
    public const int PlayerFieldNumber = 1;
    private global::Google.Protobuf.Protocol.PlayerInfo player_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Google.Protobuf.Protocol.PlayerInfo Player {
      get { return player_; }
      set {
        player_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S_EnterGame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S_EnterGame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Player, other.Player)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (player_ != null) hash ^= Player.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (player_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Player);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (player_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Player);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S_EnterGame other) {
      if (other == null) {
        return;
      }
      if (other.player_ != null) {
        if (player_ == null) {
          Player = new global::Google.Protobuf.Protocol.PlayerInfo();
        }
        Player.MergeFrom(other.Player);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (player_ == null) {
              Player = new global::Google.Protobuf.Protocol.PlayerInfo();
            }
            input.ReadMessage(Player);
            break;
          }
        }
      }
    }

  }

  public sealed partial class S_LeaveGame : pb::IMessage<S_LeaveGame> {
    private static readonly pb::MessageParser<S_LeaveGame> _parser = new pb::MessageParser<S_LeaveGame>(() => new S_LeaveGame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S_LeaveGame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_LeaveGame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_LeaveGame(S_LeaveGame other) : this() {
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_LeaveGame Clone() {
      return new S_LeaveGame(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S_LeaveGame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S_LeaveGame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S_LeaveGame other) {
      if (other == null) {
        return;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
        }
      }
    }

  }

  public sealed partial class S_Spawn : pb::IMessage<S_Spawn> {
    private static readonly pb::MessageParser<S_Spawn> _parser = new pb::MessageParser<S_Spawn>(() => new S_Spawn());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S_Spawn> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Spawn() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Spawn(S_Spawn other) : this() {
      players_ = other.players_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Spawn Clone() {
      return new S_Spawn(this);
    }

    /// <summary>Field number for the "players" field.</summary>
    public const int PlayersFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Google.Protobuf.Protocol.PlayerInfo> _repeated_players_codec
        = pb::FieldCodec.ForMessage(10, global::Google.Protobuf.Protocol.PlayerInfo.Parser);
    private readonly pbc::RepeatedField<global::Google.Protobuf.Protocol.PlayerInfo> players_ = new pbc::RepeatedField<global::Google.Protobuf.Protocol.PlayerInfo>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Google.Protobuf.Protocol.PlayerInfo> Players {
      get { return players_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S_Spawn);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S_Spawn other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!players_.Equals(other.players_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= players_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      players_.WriteTo(output, _repeated_players_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += players_.CalculateSize(_repeated_players_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S_Spawn other) {
      if (other == null) {
        return;
      }
      players_.Add(other.players_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            players_.AddEntriesFrom(input, _repeated_players_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class S_Despawn : pb::IMessage<S_Despawn> {
    private static readonly pb::MessageParser<S_Despawn> _parser = new pb::MessageParser<S_Despawn>(() => new S_Despawn());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S_Despawn> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Despawn() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Despawn(S_Despawn other) : this() {
      playerIds_ = other.playerIds_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Despawn Clone() {
      return new S_Despawn(this);
    }

    /// <summary>Field number for the "playerIds" field.</summary>
    public const int PlayerIdsFieldNumber = 1;
    private static readonly pb::FieldCodec<int> _repeated_playerIds_codec
        = pb::FieldCodec.ForInt32(10);
    private readonly pbc::RepeatedField<int> playerIds_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> PlayerIds {
      get { return playerIds_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S_Despawn);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S_Despawn other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!playerIds_.Equals(other.playerIds_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= playerIds_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      playerIds_.WriteTo(output, _repeated_playerIds_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += playerIds_.CalculateSize(_repeated_playerIds_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S_Despawn other) {
      if (other == null) {
        return;
      }
      playerIds_.Add(other.playerIds_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10:
          case 8: {
            playerIds_.AddEntriesFrom(input, _repeated_playerIds_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class C_Move : pb::IMessage<C_Move> {
    private static readonly pb::MessageParser<C_Move> _parser = new pb::MessageParser<C_Move>(() => new C_Move());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<C_Move> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[4]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C_Move() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C_Move(C_Move other) : this() {
      posX_ = other.posX_;
      posY_ = other.posY_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C_Move Clone() {
      return new C_Move(this);
    }

    /// <summary>Field number for the "posX" field.</summary>
    public const int PosXFieldNumber = 1;
    private int posX_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PosX {
      get { return posX_; }
      set {
        posX_ = value;
      }
    }

    /// <summary>Field number for the "posY" field.</summary>
    public const int PosYFieldNumber = 2;
    private int posY_;
    /// <summary>
    /// dir도 필요할 수 있겠다
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PosY {
      get { return posY_; }
      set {
        posY_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as C_Move);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(C_Move other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PosX != other.PosX) return false;
      if (PosY != other.PosY) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (PosX != 0) hash ^= PosX.GetHashCode();
      if (PosY != 0) hash ^= PosY.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (PosX != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(PosX);
      }
      if (PosY != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(PosY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (PosX != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosX);
      }
      if (PosY != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosY);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(C_Move other) {
      if (other == null) {
        return;
      }
      if (other.PosX != 0) {
        PosX = other.PosX;
      }
      if (other.PosY != 0) {
        PosY = other.PosY;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            PosX = input.ReadInt32();
            break;
          }
          case 16: {
            PosY = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// C_Move는 어떤 ClientSession인지 알지만 
  /// Server에서 보낼 떄는 누군지 모름. 따라서 누가 이동할지 id를 넣어줌
  /// </summary>
  public sealed partial class S_Move : pb::IMessage<S_Move> {
    private static readonly pb::MessageParser<S_Move> _parser = new pb::MessageParser<S_Move>(() => new S_Move());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S_Move> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[5]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Move() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Move(S_Move other) : this() {
      playerId_ = other.playerId_;
      posX_ = other.posX_;
      posY_ = other.posY_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S_Move Clone() {
      return new S_Move(this);
    }

    /// <summary>Field number for the "playerId" field.</summary>
    public const int PlayerIdFieldNumber = 1;
    private int playerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PlayerId {
      get { return playerId_; }
      set {
        playerId_ = value;
      }
    }

    /// <summary>Field number for the "posX" field.</summary>
    public const int PosXFieldNumber = 2;
    private int posX_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PosX {
      get { return posX_; }
      set {
        posX_ = value;
      }
    }

    /// <summary>Field number for the "posY" field.</summary>
    public const int PosYFieldNumber = 3;
    private int posY_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PosY {
      get { return posY_; }
      set {
        posY_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S_Move);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S_Move other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerId != other.PlayerId) return false;
      if (PosX != other.PosX) return false;
      if (PosY != other.PosY) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerId != 0) hash ^= PlayerId.GetHashCode();
      if (PosX != 0) hash ^= PosX.GetHashCode();
      if (PosY != 0) hash ^= PosY.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (PlayerId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(PlayerId);
      }
      if (PosX != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(PosX);
      }
      if (PosY != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PosY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (PlayerId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerId);
      }
      if (PosX != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosX);
      }
      if (PosY != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosY);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S_Move other) {
      if (other == null) {
        return;
      }
      if (other.PlayerId != 0) {
        PlayerId = other.PlayerId;
      }
      if (other.PosX != 0) {
        PosX = other.PosX;
      }
      if (other.PosY != 0) {
        PosY = other.PosY;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            PlayerId = input.ReadInt32();
            break;
          }
          case 16: {
            PosX = input.ReadInt32();
            break;
          }
          case 24: {
            PosY = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class PlayerInfo : pb::IMessage<PlayerInfo> {
    private static readonly pb::MessageParser<PlayerInfo> _parser = new pb::MessageParser<PlayerInfo>(() => new PlayerInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<PlayerInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Protocol.ProtocolReflection.Descriptor.MessageTypes[6]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PlayerInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PlayerInfo(PlayerInfo other) : this() {
      playerId_ = other.playerId_;
      name_ = other.name_;
      posX_ = other.posX_;
      posY_ = other.posY_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PlayerInfo Clone() {
      return new PlayerInfo(this);
    }

    /// <summary>Field number for the "playerId" field.</summary>
    public const int PlayerIdFieldNumber = 1;
    private int playerId_;
    /// <summary>
    /// ingame id, db에 들어가는 id는 아님
    /// Server가 뜰때마다 바뀔 수 있는 식별자
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PlayerId {
      get { return playerId_; }
      set {
        playerId_ = value;
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 2;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "posX" field.</summary>
    public const int PosXFieldNumber = 3;
    private int posX_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PosX {
      get { return posX_; }
      set {
        posX_ = value;
      }
    }

    /// <summary>Field number for the "posY" field.</summary>
    public const int PosYFieldNumber = 4;
    private int posY_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PosY {
      get { return posY_; }
      set {
        posY_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as PlayerInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(PlayerInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerId != other.PlayerId) return false;
      if (Name != other.Name) return false;
      if (PosX != other.PosX) return false;
      if (PosY != other.PosY) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerId != 0) hash ^= PlayerId.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (PosX != 0) hash ^= PosX.GetHashCode();
      if (PosY != 0) hash ^= PosY.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (PlayerId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(PlayerId);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      if (PosX != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PosX);
      }
      if (PosY != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(PosY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (PlayerId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerId);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (PosX != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosX);
      }
      if (PosY != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosY);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(PlayerInfo other) {
      if (other == null) {
        return;
      }
      if (other.PlayerId != 0) {
        PlayerId = other.PlayerId;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.PosX != 0) {
        PosX = other.PosX;
      }
      if (other.PosY != 0) {
        PosY = other.PosY;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            PlayerId = input.ReadInt32();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
          case 24: {
            PosX = input.ReadInt32();
            break;
          }
          case 32: {
            PosY = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
