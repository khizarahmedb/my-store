"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, RefreshCw } from "lucide-react";
import { toast } from "sonner";

interface Product {
  id: {
    timestamp: number;
    creationTime: string;
  };
  apiId: number;
  description: string;
  price: number;
  category: string;
  image: string;
  title: string;
}

export default function ProductInventory() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [newPrice, setNewPrice] = useState<number>(0);
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const fetchProducts = async () => {
    try {
      setLoading(true);
      const response = await fetch("http://localhost:5177/products");
      if (!response.ok) {
        throw new Error("Failed to fetch products");
      }
      const data = await response.json();
      setProducts(data);
    } catch (error) {
      toast.error("Failed to fetch products. Please try again.", {
        id: "fetch-error",
        duration: 3000,
      });
      console.error("Error fetching products:", error);
    } finally {
      setLoading(false);
    }
  };

  const refreshInventory = async () => {
    try {
      setRefreshing(true);
      const response = await fetch("http://localhost:5177/products/initialize");
      if (!response.ok) {
        throw new Error("Failed to initialize products");
      }
      toast.success("Product inventory refreshed successfully", {
        id: "refresh-success",
        duration: 3000,
      });
      // Fetch products again after initialization
      await fetchProducts();
    } catch (error) {
      toast.error("Failed to refresh inventory. Please try again.", {
        id: "refresh-error",
        duration: 3000,
      });
      console.error("Error refreshing inventory:", error);
    } finally {
      setRefreshing(false);
    }
  };

  const openEditDialog = (product: Product) => {
    setEditingProduct(product);
    setNewPrice(product.price);
    setIsDialogOpen(true);
  };

  const updateProductPrice = async () => {
    if (!editingProduct) return;

    try {
      const response = await fetch(
        `http://localhost:5177/products/${editingProduct.apiId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ Price: newPrice }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to update product price");
      }

      toast.success(`Price updated for ${editingProduct.title}`, {
        id: "update-success",
        duration: 3000,
      });

      // Update the product in the local state
      setProducts(
        products.map((product) =>
          product.apiId === editingProduct.apiId
            ? { ...product, price: newPrice }
            : product
        )
      );

      setIsDialogOpen(false);
    } catch (error) {
      toast.error("Failed to update product price. Please try again.", {
        id: "update-error",
        duration: 3000,
      });
      console.error("Error updating product price:", error);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  return (
    <div className="container mx-auto py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Product Inventory</h1>
        <Button
          data-cy="refresh-button"
          onClick={refreshInventory}
          disabled={refreshing}
          className="flex items-center gap-2"
        >
          {refreshing ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <RefreshCw className="h-4 w-4" />
          )}
          Refresh Product Inventory
        </Button>
      </div>

      {loading ? (
        <div
          data-cy="loading-indicator"
          className="flex justify-center items-center h-64"
        >
          <Loader2 className="h-8 w-8 animate-spin" />
          <span className="ml-2">Loading products...</span>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {products.map((product) => (
            <Card
              key={product.apiId}
              data-cy="product-card"
              className="flex flex-col h-full"
            >
              <CardHeader>
                <CardTitle
                  data-cy="product-title"
                  className="line-clamp-2 h-14"
                >
                  {product.title}
                </CardTitle>
              </CardHeader>
              <CardContent className="flex-grow">
                <div className="aspect-square relative mb-4 bg-gray-100 rounded-md overflow-hidden">
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={product.image || "/placeholder.svg"}
                    alt={product.title}
                    className="object-contain w-full h-full p-4"
                    data-cy="product-image"
                  />
                </div>
                <p
                  data-cy="product-description"
                  className="text-sm text-muted-foreground line-clamp-3 mb-2"
                >
                  {product.description}
                </p>
                <div className="flex justify-between items-center">
                  <span
                    data-cy="product-price"
                    className="text-lg font-semibold"
                  >
                    ${product.price.toFixed(2)}
                  </span>
                  <span
                    data-cy="product-category"
                    className="text-sm text-muted-foreground"
                  >
                    {product.category}
                  </span>
                </div>
              </CardContent>
              <CardFooter>
                <Button
                  data-cy="edit-price-button"
                  onClick={() => openEditDialog(product)}
                  variant="outline"
                  className="w-full"
                >
                  Edit Price
                </Button>
              </CardFooter>
            </Card>
          ))}
        </div>
      )}

      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent data-cy="price-dialog">
          <DialogHeader>
            <DialogTitle>Edit Product Price</DialogTitle>
          </DialogHeader>
          <div className="py-4">
            <div className="flex items-center gap-4 mb-4">
              <div className="w-16 h-16 relative bg-gray-100 rounded-md overflow-hidden">
                {editingProduct && (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img
                    src={editingProduct.image || "/placeholder.svg"}
                    alt={editingProduct.title}
                    className="object-contain w-full h-full p-2"
                    data-cy="dialog-product-image"
                  />
                )}
              </div>
              <div>
                <h3
                  data-cy="dialog-product-title"
                  className="font-medium line-clamp-2"
                >
                  {editingProduct?.title}
                </h3>
                <p
                  data-cy="current-price"
                  className="text-sm text-muted-foreground"
                >
                  Current price: ${editingProduct?.price.toFixed(2)}
                </p>
              </div>
            </div>
            <div className="space-y-2">
              <Label htmlFor="price">New Price</Label>
              <Input
                id="price"
                data-cy="new-price-input"
                type="number"
                min="0.01"
                step="0.01"
                value={newPrice}
                onChange={(e) => setNewPrice(Number.parseFloat(e.target.value))}
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              data-cy="cancel-button"
              variant="outline"
              onClick={() => setIsDialogOpen(false)}
            >
              Cancel
            </Button>
            <Button data-cy="update-price-button" onClick={updateProductPrice}>
              Update Price
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
