import { useState, useEffect } from 'react';
import { Plus, Search, Bell, Menu } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { SettlementCard } from '@/components/SettlementCard';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { settlementsApi, removeAuthToken } from '@/lib/api';

// Mock data for development
const mockSettlements = [
  {
    id: '1',
    name: 'Weekend Trip',
    participantCount: 4,
    totalExpenses: 450.00,
    lastActivity: '2 days ago',
    userBalance: 75.50,
  },
  {
    id: '2',
    name: 'Dinner Split',
    participantCount: 3,
    totalExpenses: 120.00,
    lastActivity: '1 week ago',
    userBalance: -40.00,
  },
  {
    id: '3',
    name: 'House Expenses',
    participantCount: 2,
    totalExpenses: 800.00,
    lastActivity: '3 days ago',
    userBalance: 0,
  },
];

interface DashboardProps {
  onNavigate: (page: string, data?: any) => void;
  onLogout: () => void;
}

export function Dashboard({ onNavigate, onLogout }: DashboardProps) {
  const [settlements, setSettlements] = useState(mockSettlements);
  const [searchQuery, setSearchQuery] = useState('');
  const [loading, setLoading] = useState(false);
  const { toast } = useToast();

  const filteredSettlements = settlements.filter(settlement =>
    settlement.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const totalOwed = settlements.reduce((sum, s) => sum + Math.max(0, s.userBalance), 0);
  const totalOwing = Math.abs(settlements.reduce((sum, s) => sum + Math.min(0, s.userBalance), 0));

  const handleLogout = () => {
    removeAuthToken();
    onLogout();
    toast({
      title: "Logged out",
      description: "See you next time!",
    });
  };

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <div className="bg-white border-b border-border p-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-gradient-primary rounded-full flex items-center justify-center">
              <span className="text-lg font-bold text-white">ES</span>
            </div>
            <div>
              <h1 className="text-xl font-bold text-foreground">ExpenseSplitter</h1>
              <p className="text-sm text-muted-foreground">Welcome back!</p>
            </div>
          </div>
          
          <div className="flex items-center gap-2">
            <Button variant="ghost" size="icon" className="relative">
              <Bell className="w-5 h-5" />
              <div className="absolute -top-1 -right-1 w-3 h-3 bg-destructive rounded-full" />
            </Button>
            <Button variant="ghost" size="icon" onClick={handleLogout}>
              <Menu className="w-5 h-5" />
            </Button>
          </div>
        </div>
      </div>

      <div className="p-4 space-y-6">
        {/* Balance Summary */}
        <div className="grid grid-cols-2 gap-4">
          <Card className="shadow-card border-0">
            <CardContent className="p-4">
              <div className="text-center">
                <div className="text-2xl font-bold text-green-600">+${totalOwed.toFixed(2)}</div>
                <div className="text-sm text-muted-foreground">You are owed</div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="shadow-card border-0">
            <CardContent className="p-4">
              <div className="text-center">
                <div className="text-2xl font-bold text-red-600">${totalOwing.toFixed(2)}</div>
                <div className="text-sm text-muted-foreground">You owe</div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Quick Actions */}
        <div className="grid grid-cols-2 gap-4">
          <Button 
            onClick={() => onNavigate('createSettlement')}
            className="h-16 flex-col gap-2"
          >
            <Plus className="w-6 h-6" />
            <span>New Settlement</span>
          </Button>
          
          <Button 
            variant="secondary"
            onClick={() => onNavigate('joinSettlement')}
            className="h-16 flex-col gap-2"
          >
            <Search className="w-6 h-6" />
            <span>Join Settlement</span>
          </Button>
        </div>

        {/* Search */}
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-muted-foreground" />
          <Input
            type="text"
            placeholder="Search settlements..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-10 h-12 rounded-xl border-border"
          />
        </div>

        {/* Settlements List */}
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-foreground">Your Settlements</h2>
            <span className="text-sm text-muted-foreground">{filteredSettlements.length} settlements</span>
          </div>
          
          {filteredSettlements.length === 0 ? (
            <Card className="shadow-card border-0">
              <CardContent className="p-8 text-center">
                <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center mx-auto mb-4">
                  <Search className="w-8 h-8 text-muted-foreground" />
                </div>
                <h3 className="font-semibold mb-2">No settlements found</h3>
                <p className="text-sm text-muted-foreground mb-4">
                  {searchQuery ? 'Try a different search term' : 'Create your first settlement to get started'}
                </p>
                {!searchQuery && (
                  <Button onClick={() => onNavigate('createSettlement')}>
                    <Plus className="mr-2 w-4 h-4" />
                    Create Settlement
                  </Button>
                )}
              </CardContent>
            </Card>
          ) : (
            filteredSettlements.map(settlement => (
              <SettlementCard
                key={settlement.id}
                settlement={settlement}
                onClick={() => onNavigate('settlement', { id: settlement.id })}
              />
            ))
          )}
        </div>
      </div>
    </div>
  );
}