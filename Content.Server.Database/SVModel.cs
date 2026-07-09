using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database;

public static class SVModel
{
    [Table("sv_profiles")]
    public class SVProfile
    {
        [Key]
        public int ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; } = null!;

        /// <summary>
        /// The player's username. Updated on every spawn.
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        /// The character's name. Can change between rounds.
        /// </summary>
        public string CharacterName { get; set; } = string.Empty;

        /// <summary>
        /// Lobby-authored character flavour data serialized as JSON
        /// (height, weight, emergency contact, allergies, etc.).
        /// </summary>
        [Column(TypeName = "jsonb")]
        public JsonDocument? CharacterDocumentGeneral { get; set; }

        [JsonIgnore]
        public List<CharacterDocument> CharacterDocuments { get; set; } = new();
    }

    [Table("sv_character_document_entries"), Index(nameof(DocID))]
    public class CharacterDocument
    {
        [Key]
        public int DocID { get; set; }
        public int DocType { get; set; }
        public DateTime DocDateLastEdited { get; set; }
        public string DocTitle { get; set; } = string.Empty;
        public string DocAuthor { get; set; } = string.Empty;
        public string DocLastEditedBy { get; set; } = string.Empty;
        public string DocContent { get; set; } = string.Empty;
        public string DocStamps { get; set; } = string.Empty;

        /// <summary>
        /// When non-null, this document has been soft-deleted ("binned"). It stays in the
        /// table so Central Command / admins can review or restore it, and is permanently
        /// purged by <see cref="ServerDbBase.PurgeExpiredSVCharacterDocumentsAsync"/> once
        /// it is older than the configured retention window. Stored as UTC.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public SVProfile SVProfile { get; set; } = null!;
        public int ProfileId { get; set; }
    }
}
