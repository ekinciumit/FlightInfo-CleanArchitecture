import React, { useState, useEffect } from 'react';
import './TwoFactorVerify.css';

interface TwoFactorVerifyProps {
  userId: number;
  twoFactorType: 'SMS' | 'Email';
  phoneNumber?: string;
  onVerificationSuccess: (token: string, user: any) => void;
}

const TwoFactorVerify: React.FC<TwoFactorVerifyProps> = ({
  userId,
  twoFactorType,
  phoneNumber,
  onVerificationSuccess
}) => {
  const [verificationCode, setVerificationCode] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [timeLeft, setTimeLeft] = useState(300); // 5 dakika
  const [resendCount, setResendCount] = useState(0); // Tekrar gönder sayacı
  const [resendCooldown, setResendCooldown] = useState(0); // 30 saniye bekleme
  const maxResends = 2; // İlk 2 kez beklemeden gönder

    useEffect(() => {
        if (timeLeft > 0) {
            const timer = setTimeout(() => setTimeLeft(timeLeft - 1), 1000);
            return () => clearTimeout(timer);
        }
    }, [timeLeft]);

    // 30 saniye bekleme timer'ı
    useEffect(() => {
        if (resendCooldown > 0) {
            const timer = setTimeout(() => setResendCooldown(resendCooldown - 1), 1000);
            return () => clearTimeout(timer);
        }
    }, [resendCooldown]);

    // Sayfa yüklendiğinde otomatik email gönder
    useEffect(() => {
        const sendInitialEmail = async () => {
            try {
                const response = await fetch('http://localhost:7104/api/Auth/send-email-code', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        userId
                    }),
                });

                const data = await response.json();
                if (response.ok) {
                    setMessage('Email kodu gönderildi');
                } else {
                    setMessage(data.message || 'Email kodu gönderilemedi');
                }
            } catch (error) {
                console.error('Error sending initial email code:', error);
            }
        };

        sendInitialEmail();
    }, [userId]);

  const handleVerifyCode = async () => {
    if (!verificationCode.trim()) {
      setMessage('Doğrulama kodu gerekli');
      return;
    }

    if (verificationCode.length !== 6) {
      setMessage('Kod 6 haneli olmalıdır');
      return;
    }

    setIsLoading(true);
    setMessage('');

    try {
      const response = await fetch('http://localhost:7104/api/Auth/verify-2fa-login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userId,
          code: verificationCode
        }),
      });

      const data = await response.json();

      // Debug için API response'unu kontrol et
      console.log('🔍 API RESPONSE:', data);
      console.log('🔍 TOKEN VAR MI:', data.token ? 'EVET' : 'HAYIR');
      console.log('🔍 USER VAR MI:', data.user ? 'EVET' : 'HAYIR');
      console.log('🔍 RESPONSE KEYS:', Object.keys(data));
      console.log('🔍 RESPONSE VALUES:', Object.values(data));

      // Sadece geçerli token ve user varsa başarılı say
      if (response.ok && data.token && data.token.length > 0 && data.user && data.user !== null) {
        setMessage('Doğrulama başarılı! Giriş yapılıyor...');

        // Token'ı localStorage'a kaydet
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));

        // Debug için console'a yazdır
        console.log('🔑 TOKEN KAYDEDİLDİ:', data.token);
        console.log('👤 USER KAYDEDİLDİ:', data.user);

        // Auth state'i güncelle
        window.dispatchEvent(new Event('authChange'));

        // Parent callback'i tetikle
        onVerificationSuccess(data.token, data.user);
      } else {
        setMessage(data.message || 'Kod doğrulama başarısız');
      }
    } catch (error) {
      setMessage('Bir hata oluştu');
      console.error('Error verifying code:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendCode = async () => {
    // 30 saniye bekleme kontrolü
    if (resendCooldown > 0) {
      setMessage(`Lütfen ${resendCooldown} saniye bekleyin`);
      return;
    }

    setIsLoading(true);
    setMessage('');

    try {
      let response;
      if (twoFactorType === 'SMS') {
        response = await fetch('http://localhost:7104/api/Auth/send-sms-code', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId,
            phoneNumber
          }),
        });
      } else {
        response = await fetch('http://localhost:7104/api/Auth/send-email-code', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId
          }),
        });
      }

      const data = await response.json();

      if (response.ok) {
        setMessage('Yeni kod gönderildi');
        setTimeLeft(300);
        setVerificationCode('');
        setResendCount(prev => prev + 1);

        // 2'den sonra 30 saniye bekleme başlat
        if (resendCount >= maxResends) {
          setResendCooldown(30);
        }
      } else {
        setMessage(data.message || 'Kod gönderilemedi');
      }
    } catch (error) {
      setMessage('Bir hata oluştu');
      console.error('Error resending code:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleBackToLogin = () => {
    window.location.href = '/login';
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  return (
    <div className="two-factor-verify">
      <div className="verify-container">
        <div className="verify-header">
          <div className="verify-icon">
            {twoFactorType === 'SMS' ? '📱' : '📧'}
          </div>
          <h2>İki Faktörlü Doğrulama</h2>
          <p>
            {twoFactorType === 'SMS'
              ? `${phoneNumber} numarasına gönderilen 6 haneli kodu girin`
              : 'Email adresinize gönderilen 6 haneli kodu girin'
            }
          </p>
        </div>

        <div className="verification-form">
          <div className="code-input-container">
            <input
              type="text"
              value={verificationCode}
              onChange={(e) => {
                const value = e.target.value.replace(/\D/g, '').slice(0, 6);
                setVerificationCode(value);
              }}
              placeholder="123456"
              maxLength={6}
              className="code-input"
              autoComplete="one-time-code"
            />
            <div className="code-dots">
              {Array.from({ length: 6 }, (_, i) => (
                <div
                  key={i}
                  className={`code-dot ${verificationCode.length > i ? 'filled' : ''}`}
                />
              ))}
            </div>
          </div>

          {timeLeft > 0 && (
            <div className="timer">
              <span className="timer-icon">⏱️</span>
              <span>Kod {formatTime(timeLeft)} sonra geçersiz olacak</span>
            </div>
          )}

          {message && (
            <div className={`message ${message.includes('başarılı') ? 'success' : 'error'}`}>
              {message}
            </div>
          )}

          <div className="button-group">
            <button
              onClick={handleVerifyCode}
              disabled={isLoading || verificationCode.length !== 6}
              className="btn-primary"
            >
              {isLoading ? 'Doğrulanıyor...' : 'Doğrula'}
            </button>

                <button
                  onClick={handleResendCode}
                  disabled={isLoading || resendCooldown > 0}
                  className="btn-secondary"
                >
                  {resendCooldown > 0
                    ? `Tekrar gönder (${resendCooldown}s)`
                    : 'Kodu Tekrar Gönder'
                  }
                </button>

            <button
              onClick={handleBackToLogin}
              className="btn-link"
            >
              Giriş sayfasına dön
            </button>
          </div>
        </div>

        <div className="help-text">
          <p>
            <strong>Kod almadınız mı?</strong>
          </p>
          <ul>
            <li>Spam klasörünü kontrol edin</li>
            <li>Telefon sinyalinizi kontrol edin</li>
            <li>Kod gönderme işlemini tekrar deneyin</li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default TwoFactorVerify;

