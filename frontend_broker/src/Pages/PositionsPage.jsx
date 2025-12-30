import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../api/api';

const PositionsPage = () => {
    const [positions, setPositions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const navigate = useNavigate();

    const getProfitColor = (val) => val > 0 ? 'text-success' : (val < 0 ? 'text-danger' : 'text-secondary');

    useEffect(() => {
        const fetchPositions = async () => {
            try {
                const data = await api.portfolio.get();
                setPositions(data.positions );
            } catch (err) {
                setError('Nie udało się pobrać listy akcji.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        fetchPositions();
    }, []);

    if (loading) {
        return (
            <div className="d-flex justify-content-center align-items-center w-100 py-5">
                <div className="spinner-border text-primary me-3" role="status"></div>
                <span className="text-secondary fw-bold">Ładowanie pozycji...</span>
            </div>
        );
    }

    if (error) {
        return (
            <div className="container mt-5">
                <div className="alert alert-danger text-center" role="alert">
                    <i className="bi bi-exclamation-triangle-fill me-2"></i>
                    {error}
                </div>
            </div>
        );
    }

    return (
        <div className="w-100">

            <div className="mb-4 border-bottom pb-3">
                <h2 className="text-dark fw-bold mb-0">Twoje Akcje</h2>
            </div>

            <div className="card shadow-sm border-0 rounded-3 overflow-hidden">
                <div className="card-header bg-white py-3 border-bottom">
                    <h5 className="mb-0 fw-bold text-secondary">Lista Pozycji</h5>
                </div>

                <div className="card-body p-0">
                    {!positions || positions.length === 0 ? (
                        <div className="p-5 text-center text-muted">
                            <p className="mb-3">Nie posiadasz jeszcze żadnych akcji.</p>
                            <button
                                onClick={() => navigate('/market')}
                                className="btn btn-primary fw-bold"
                            >
                                Przejdź do Rynku
                            </button>
                            <span className="ms-2">aby zainwestować.</span>
                        </div>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-hover align-middle mb-0">
                                <thead className="table-light">
                                    <tr>
                                        <th scope="col" className="py-3 ps-4 text-secondary small text-uppercase">Symbol</th>
                                        <th scope="col" className="py-3 text-secondary small text-uppercase">Ilość</th>
                                        <th scope="col" className="py-3 text-secondary small text-uppercase">Śr. Cena</th>
                                        <th scope="col" className="py-3 text-secondary small text-uppercase">Cena Aktualna</th>
                                        <th scope="col" className="py-3 text-secondary small text-uppercase">Wartość</th>
                                        <th scope="col" className="py-3 pe-4 text-secondary small text-uppercase">Zysk/Strata</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {positions.map((pos, index) => (
                                        <tr
                                            key={index}
                                            onClick={() => navigate(`/positions/${pos.ticker}`)}
                                            style={{ cursor: 'pointer' }}
                                            className="position-relative"
                                        >
                                            <td className="ps-4 fw-bold text-primary">{pos.ticker}</td>
                                            <td className="text-dark">{pos.quantity}</td>
                                            <td className="text-muted">${pos.avgCost?.toFixed(2)}</td>
                                            <td className="fw-bold text-dark">${pos.currentPrice?.toFixed(2)}</td>
                                            <td className="fw-semibold text-dark">${pos.currentValue?.toFixed(2)}</td>
                                            <td className={`pe-4 fw-bold ${getProfitColor(pos.profitLoss)}`}>
                                                {pos.profitLoss > 0 ? '+' : ''}{pos.profitLoss?.toFixed(2)}$
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default PositionsPage;