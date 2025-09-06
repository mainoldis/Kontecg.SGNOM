namespace Kontecg.Accounting
{
    /// <summary>
    /// Defines the lifecycle states of accounting notes and vouchers in the accounting workflow.
    /// This enumeration represents the various stages that an accounting document can go through
    /// from creation to final processing.
    /// </summary>
    /// <remarks>
    /// The status values are used to track the progress of accounting entries through the
    /// approval and processing workflow. Each status indicates a specific stage in the
    /// document lifecycle and may trigger different business rules or user permissions.
    /// </remarks>
    public enum AccountingNoteStatus
    {
        /// <summary>
        /// Indicates that the accounting note requires analysis and review before processing.
        /// </summary>
        /// <remarks>
        /// This is typically the initial state for new accounting entries that need
        /// validation, approval, or additional information before being processed.
        /// </remarks>
        ToAnalyze = 0,

        /// <summary>
        /// Indicates that the accounting note is approved and ready to be created/processed.
        /// </summary>
        /// <remarks>
        /// This status indicates that all necessary approvals have been obtained and
        /// the entry is ready for final processing in the accounting system.
        /// </remarks>
        ToMake = 1,

        /// <summary>
        /// Indicates that the accounting note has been successfully created and processed.
        /// </summary>
        /// <remarks>
        /// This is the final state for successfully processed accounting entries.
        /// The entry has been posted to the general ledger and is part of the official records.
        /// </remarks>
        Made = 2,

        /// <summary>
        /// Indicates that the accounting note has not been claimed or assigned for processing.
        /// </summary>
        /// <remarks>
        /// This status is used when entries are waiting to be assigned to specific
        /// users or departments for processing.
        /// </remarks>
        Unclaimed = 3,

        /// <summary>
        /// Indicates that the accounting note has been claimed and is being processed.
        /// </summary>
        /// <remarks>
        /// This status indicates that a user or department has taken responsibility
        /// for processing the accounting entry.
        /// </remarks>
        Claimed = 4,

        /// <summary>
        /// Indicates that the accounting note is in a pending state awaiting further action.
        /// </summary>
        /// <remarks>
        /// This status is used when entries are waiting for additional information,
        /// approvals, or external dependencies before processing can continue.
        /// </remarks>
        Pending = 5,

        /// <summary>
        /// Indicates that the accounting note is marked for deletion but not yet removed.
        /// </summary>
        /// <remarks>
        /// This status is used for entries that have been approved for deletion
        /// but are still in the system for audit purposes or pending final removal.
        /// </remarks>
        PendingToDelete = 6,

        /// <summary>
        /// Indicates that the accounting note has been cancelled and is no longer valid.
        /// </summary>
        /// <remarks>
        /// Cancelled entries are retained in the system for audit purposes but are
        /// not included in financial calculations or reports.
        /// </remarks>
        Cancelled = 7,

        /// <summary>
        /// Indicates that the accounting note has been contributed or submitted for processing.
        /// </summary>
        /// <remarks>
        /// This status indicates that the entry has been submitted by a user and
        /// is now in the workflow for review and processing.
        /// </remarks>
        Contributed = 8,
    }
}
