import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../api/api';
import TransactionModal from '../components/TransactionModal';

const PortfolioPage = () => {
    const [portfolio, setPortfolio] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const [modalOpen, setModalOpen] = useState(false);
    const [transactionType, setTransactionType] = useState('deposit'); 
    const [transactionLoading, setTransactionLoading] = useState(false);
    const [transactionError, setTransactionError] = useState('');

    const fetchPortfolio = async () => {
        if (!localStorage.getItem('token')) {
            navigate('/login');
            return;
        }
        try {
            const data = await api.portfolio.get();
            setPortfolio(data);
        } catch (err) {
            setError(err.message || 'Błąd pobierania danych');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPortfolio();
    }, [navigate]);

    const openModal = (type) => {
        setTransactionType(type);
        setTransactionError('');
        setModalOpen(true);
    };

    const handleTransaction = async (amount) => {
        setTransactionLoading(true);
        setTransactionError('');
        try {
            if (transactionType === 'deposit') {
                await api.wallet.deposit(amount);
            } else {
                await api.wallet.withdraw(amount);
            }
            setModalOpen(false);
            setLoading(true);
            await fetchPortfolio();
        } catch (err) {
            setTransactionError(err.response.data || err.message);
        } finally {
            setTransactionLoading(false);
        }
    };


    if (loading && !portfolio) {
        return (
            <div className="d-flex justify-content-center align-items-center w-100 py-5">
                <div className="spinner-border text-primary me-2" role="status"></div>
                <span className="text-secondary">Ładowanie portfela...</span>
            </div>
        );
    }

    if (error) {
        return (
            <div className="alert alert-danger text-center mt-5 mx-auto" style={{ maxWidth: '600px' }}>
                {error}
            </div>
        );
    }

    return (
        <div className="w-100">

            <div className="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3">
                <h2 className="text-dark fw-bold m-0">Twój Portfel</h2>
            </div>

            {portfolio && (
                <>
                    <div className="row g-4 mb-5">

                        <div className="col-md-4">
                            <div className="card h-100 shadow-sm border-0 border-start border-4 border-primary">
                                <div className="card-body">
                                    <h6 className="text-muted text-uppercase small fw-bold mb-2">Wartość Całkowita</h6>
                                    <h2 className="card-title fw-bold text-dark mb-0">
                                        ${portfolio.totalPortfolioValue?.toFixed(2)}
                                    </h2>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="card h-100 shadow-sm border-0 border-start border-4 border-success">
                                <div className="card-body d-flex flex-column justify-content-between">
                                    <div>
                                        <h6 className="text-muted text-uppercase small fw-bold mb-2">Dostępna Gotówka</h6>
                                        <h2 className="card-title fw-bold text-dark mb-3">
                                            ${portfolio.cashBalance?.toFixed(2)}
                                        </h2>
                                    </div>

                                    <div className="d-flex gap-2 mt-auto">
                                        <button
                                            onClick={() => openModal('deposit')}
                                            className="btn btn-success flex-fill fw-bold btn-sm py-2"
                                        >
                                            + Wpłać
                                        </button>
                                        <button
                                            onClick={() => openModal('withdraw')}
                                            className="btn btn-outline-danger flex-fill fw-bold btn-sm py-2"
                                        >
                                            - Wypłać
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="card h-100 shadow-sm border-0 border-start border-4" style={{ borderLeftColor: '#6f42c1' }}>
                                <div className="card-body">
                                    <h6 className="text-muted text-uppercase small fw-bold mb-2">Wartość Akcji</h6>
                                    <h2 className="card-title fw-bold text-dark mb-0">
                                        ${portfolio.totalAssetsValue?.toFixed(2)}
                                    </h2>
                                </div>
                            </div>
                        </div>
                    </div>

                   
                </>
            )}

            <TransactionModal
                isOpen={modalOpen}
                onClose={() => setModalOpen(false)}
                type={transactionType}
                onConfirm={handleTransaction}
                loading={transactionLoading}
                error={transactionError}
            />
        </div>
    );
};

export default PortfolioPage;