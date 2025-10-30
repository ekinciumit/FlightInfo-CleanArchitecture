import React from 'react';
import './ConfirmDialog.css';

export interface ConfirmDialogProps {
    isOpen: boolean;
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    onConfirm: () => void;
    onCancel: () => void;
    type?: 'warning' | 'danger' | 'info';
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
    isOpen,
    title,
    message,
    confirmText = "Evet",
    cancelText = "İptal",
    onConfirm,
    onCancel,
    type = 'warning'
}) => {
    if (!isOpen) return null;

    const handleBackdropClick = (e: React.MouseEvent) => {
        if (e.target === e.currentTarget) {
            onCancel();
        }
    };

    return (
        <div className="confirm-dialog-backdrop" onClick={handleBackdropClick}>
            <div className={`confirm-dialog ${type}`}>
                <div className="confirm-dialog-header">
                    <div className="confirm-dialog-icon">
                        {type === 'warning' && '⚠️'}
                        {type === 'danger' && '🗑️'}
                        {type === 'info' && 'ℹ️'}
                    </div>
                    <h3 className="confirm-dialog-title">{title}</h3>
                </div>

                <div className="confirm-dialog-body">
                    <p className="confirm-dialog-message">{message}</p>
                </div>

                <div className="confirm-dialog-footer">
                    <button
                        className="confirm-dialog-button cancel"
                        onClick={onCancel}
                    >
                        {cancelText}
                    </button>
                    <button
                        className={`confirm-dialog-button confirm ${type}`}
                        onClick={onConfirm}
                    >
                        {confirmText}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ConfirmDialog;


