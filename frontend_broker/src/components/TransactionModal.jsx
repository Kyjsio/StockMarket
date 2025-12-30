import React, { useState } from 'react';

const TransactionModal = ({ isOpen, onClose, type, onConfirm, loading, error }) => {
    const [amount, setAmount] = useState('');

    if (!isOpen) return null;

    const handleSubmit = (e) => {
        e.preventDefault();
        onConfirm(parseFloat(amount));
        setAmount('');
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md shadow-xl">
                <h3 className="text-xl font-bold mb-4 text-gray-800">
                    {type === 'deposit' ? 'Wpłać środki' : 'Wypłać środki'}
                </h3>

                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-700 mb-1">Kwota ($)</label>
                        <input
                            type="number"
                            step="0.01"
                            min="0.01"
                            required
                            className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={amount}
                            onChange={(e) => setAmount(e.target.value)}
                            placeholder="0.00"
                        />
                    </div>

                    {error && (
                        <div className="mb-4 p-2 bg-red-50 text-red-600 text-sm rounded border border-red-200">
                            {error}
                        </div>
                    )}

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={onClose}
                            className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-md transition-colors"
                            disabled={loading}
                        >
                            Anuluj
                        </button>
                        <button
                            type="submit"
                            className={`px-4 py-2 rounded-md text-white font-medium transition-colors ${type === 'deposit'
                                    ? 'bg-green-600 hover:bg-green-700'
                                    : 'bg-red-600 hover:bg-red-700'
                                } ${loading ? 'opacity-50 cursor-not-allowed' : ''}`}
                            disabled={loading}
                        >
                            {loading ? 'Przetwarzanie...' : (type === 'deposit' ? 'Wpłać' : 'Wypłać')}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default TransactionModal;