import React, { useEffect, useState } from 'react';
import './Toast.css';

export interface ToastProps {
    message: string;
    type: 'success' | 'error' | 'info' | 'warning';
    duration?: number;
    onClose: () => void;
}

const Toast: React.FC<ToastProps> = ({ message, type, duration = 3000, onClose }) => {
    const [isVisible, setIsVisible] = useState(true);

    useEffect(() => {
        const timer = setTimeout(() => {
            setIsVisible(false);
            setTimeout(onClose, 300); // CSS transition için bekle
        }, duration);

        return () => clearTimeout(timer);
    }, [duration, onClose]);

    const handleClose = () => {
        setIsVisible(false);
        setTimeout(onClose, 300);
    };

    return (
        <div className={`toast ${type} ${isVisible ? 'show' : 'hide'}`}>
            <div className="toast-content">
                <div className="toast-icon">
                    {type === 'success' && '✓'}
                    {type === 'error' && '✗'}
                    {type === 'info' && 'ℹ'}
                    {type === 'warning' && '⚠'}
                </div>
                <div className="toast-message">{message}</div>
                <button className="toast-close" onClick={handleClose}>
                    ×
                </button>
            </div>
        </div>
    );
};

export default Toast;
