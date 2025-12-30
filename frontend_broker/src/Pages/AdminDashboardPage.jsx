import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api/api';

const AdminDashboardPage = () => {
    const [formData, setFormData] = useState({ ticker: '', fullName: '' });
    const [formMessage, setFormMessage] = useState(null);
    const [formError, setFormError] = useState(null);
    const [formLoading, setFormLoading] = useState(false);

    const [stats, setStats] = useState(null);
    const [statsLoading, setStatsLoading] = useState(true);

    useEffect(() => {
        loadStats();
    }, []);

    const loadStats = async () => {
        try {
            const data = await api.admin.getSystemStats();
            setStats(data);
        } catch (err) {
            console.error("Błąd pobierania statystyk:", err);
        } finally {
            setStatsLoading(false);
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFormLoading(true);
        setFormMessage(null);
        setFormError(null);
        try {
            const response = await api.admin.addAsset(formData);
            setFormMessage(response.message);
            setFormData({ ticker: '', fullName: '' });
        } catch (err) {
            setFormError(err.message);
        } finally {
            setFormLoading(false);
        }
    };

    const StatCard = ({ title, value, icon, color }) => (
        <div className="col-md-6 col-lg-4">
            <div className={`card shadow-sm border-0 border-start border-4 border-${color} h-100`}>
                <div className="card-body">
                    <div className="d-flex align-items-center justify-content-between">
                        <div>
                            <div className="text-muted small text-uppercase fw-bold mb-1">{title}</div>
                            <div className="h4 mb-0 fw-bold text-dark">
                                {statsLoading ? <div className="spinner-border spinner-border-sm"></div> : value}
                            </div>
                        </div>
                        <div className={`text-${color} bg-${color} bg-opacity-10 rounded p-3`}>
                            <i className={`bi ${icon} fs-4`}></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );

    return (
        <div className="w-100 animate-fade-in">
            <div className="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3 mt-4">
                <div>
                    <h2 className="h2 text-dark fw-bold mb-0 mt-4">Panel Administratora</h2>
                    <p className="text-muted mb-0">Przegląd systemu i zarządzanie rynkiem</p>
                </div>
            </div>

            <div className="row g-4 mb-5">
                <StatCard
                    title="Aktywni Inwestorzy"
                    value={stats?.totalUsers || 0}
                    icon="bi-people-fill"
                    color="primary"
                />
                <StatCard
                    title="Kapitał w Systemie"
                    value={`$${stats?.totalSystemCash?.toLocaleString('en-US', { minimumFractionDigits: 2 }) || '0.00'}`}
                    icon="bi-cash-stack"
                    color="success"
                />
                <StatCard
                    title="Wykonane Transakcje"
                    value={stats?.totalTradesExecuted || 0}
                    icon="bi-receipt"
                    color="info"
                />
            </div>

            <div className="row g-4">
                <div className="col-lg-5">
                    <div className="card shadow-lg border-0 rounded-3 h-100">
                        <div className="card-body p-4">
                            <h4 className="card-title fw-bold text-dark mb-4 border-bottom pb-2">
                                <i className="bi bi-plus-circle-fill text-primary me-2"></i>
                                Dodaj Spółkę do Rynku
                            </h4>

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label className="form-label fw-bold text-secondary">Symbol (Ticker)</label>
                                    <input
                                        type="text"
                                        name="ticker"
                                        value={formData.ticker}
                                        onChange={handleChange}
                                        placeholder="np. AAPL"
                                        className="form-control form-control-lg text-uppercase font-monospace"
                                        required
                                        maxLength={10}
                                    />
                                    <div className="form-text">Symbol zgodny z AlphaVantage API.</div>
                                </div>

                                <div className="mb-4">
                                    <label className="form-label fw-bold text-secondary">Nazwa Spółki</label>
                                    <input
                                        type="text"
                                        name="fullName"
                                        value={formData.fullName}
                                        onChange={handleChange}
                                        placeholder="np. Apple Inc."
                                        className="form-control form-control-lg"
                                        required
                                    />
                                </div>

                                {formMessage && (
                                    <div className="alert alert-success d-flex align-items-center mb-3" role="alert">
                                        <i className="bi bi-check-circle-fill me-2"></i>
                                        <div>{formMessage}</div>
                                    </div>
                                )}
                                {formError && (
                                    <div className="alert alert-danger d-flex align-items-center mb-3" role="alert">
                                        <i className="bi bi-exclamation-triangle-fill me-2"></i>
                                        <div>{formError}</div>
                                    </div>
                                )}

                                <button
                                    type="submit"
                                    disabled={formLoading}
                                    className="btn btn-primary btn-lg w-100 fw-bold shadow-sm"
                                >
                                    {formLoading ? <span className="spinner-border spinner-border-sm me-2"></span> : null}
                                    {formLoading ? 'Dodawanie...' : 'Dodaj Walor'}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>

                <div className="col-lg-7">
                    <div className="h-100 p-5 bg-white border rounded-3 shadow-sm d-flex flex-column justify-content-center">
                        <div className="text-center mb-4">
                            <i className="bi bi-shield-lock display-1 text-primary opacity-25"></i>
                            <h3 className="text-secondary fw-light mt-3">Centrum Zarządzania</h3>
                            <p className="text-muted">Wybierz moduł, którym chcesz zarządzać</p>
                        </div>

                        <div className="d-grid gap-3 d-sm-flex justify-content-center">
                            <Link to="/admin/users" className="btn btn-outline-primary btn-lg px-4 d-flex align-items-center justify-content-center gap-2">
                                <i className="bi bi-people"></i>
                                Zarządzaj Użytkownikami
                            </Link>
                            <Link to="/admin/reports" className="btn btn-outline-dark btn-lg px-4 d-flex align-items-center justify-content-center gap-2">
                                <i className="bi bi-file-earmark-bar-graph"></i>
                                Raporty Finansowe
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboardPage;